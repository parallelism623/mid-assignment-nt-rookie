using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

using MIDASM.Persistence.Services;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using MIDASM.Application.Services.AuditLogServices;
using MockQueryable;

namespace MIDASM.Persistence.Services.Tests
{
    public class ReportServicesTests
    {
        private readonly Mock<IBookRepository> _bookRepo;
        private readonly Mock<IBookBorrowingRequestDetailRepository> _detailRepo;
        private readonly Mock<IUserRepository> _userRepo;
        private readonly Mock<IAuditLogger> _auditLogger;
        private readonly Mock<ICategoryRepository> _categoryRepo;
        private readonly Mock<IBookBorrowingRequestRepository> _borrowReqRepo;
        private readonly ReportServices _service;

        public ReportServicesTests()
        {
            _bookRepo = new Mock<IBookRepository>();
            _detailRepo = new Mock<IBookBorrowingRequestDetailRepository>();
            _userRepo = new Mock<IUserRepository>();
            _auditLogger = new Mock<IAuditLogger>();
            _categoryRepo = new Mock<ICategoryRepository>();
            _borrowReqRepo = new Mock<IBookBorrowingRequestRepository>();

            _service = new ReportServices(
                _bookRepo.Object,
                _detailRepo.Object,
                _userRepo.Object,
                _auditLogger.Object,
                _categoryRepo.Object,
                _borrowReqRepo.Object
            );
        }

        [Fact]
        public async Task GetBookBorrowingReportAsync_NoData_ReturnsEmpty()
        {
            // Arrange
            var books = new List<Book>().AsQueryable().BuildMock();
            var details = new List<BookBorrowingRequestDetail>().AsQueryable().BuildMock();
            _bookRepo.Setup(r => r.GetQueryable()).Returns(books);
            _detailRepo.Setup(r => r.GetQueryable()).Returns(details);

            // Act
            var result = await _service.GetBookBorrowingReportAsync(
                new BookBorrowingReportQueryParameters
                {
                    FromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                    ToDate = DateOnly.FromDateTime(DateTime.Today),
                    Top = 5
                }
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBookBorrowingReportAsync_WithData_GroupsAndLimits()
        {
            // Arrange
            var b1 = new Book { Id = Guid.NewGuid(), Title = "T1", Author = "A1", Quantity = 5, Available = 2, Category = new Category { Name = "Cat1" } };
            var b2 = new Book { Id = Guid.NewGuid(), Title = "T2", Author = "A2", Quantity = 3, Available = 1, Category = new Category { Name = "Cat2" } };
            var books = new List<Book> { b1, b2 }.AsQueryable().BuildMock();
            _bookRepo.Setup(r => r.GetQueryable()).Returns(books);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var dlist = new List<BookBorrowingRequestDetail>
            {
                new BookBorrowingRequestDetail { BookId = b1.Id, BookBorrowingRequest = new BookBorrowingRequest { DateRequested = today } },
                new BookBorrowingRequestDetail { BookId = b1.Id, BookBorrowingRequest = new BookBorrowingRequest { DateRequested = today } },
                new BookBorrowingRequestDetail { BookId = b2.Id, BookBorrowingRequest = new BookBorrowingRequest { DateRequested = today } }
            }.AsQueryable().BuildMock();
            _detailRepo.Setup(r => r.GetQueryable()).Returns(dlist);

            // Act
            var result = await _service.GetBookBorrowingReportAsync(
                new BookBorrowingReportQueryParameters { FromDate = today.AddDays(-1), ToDate = today.AddDays(1), Top = 1 }
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Items.Should().HaveCount(1);
            var top = result.Data.Items.First();
            top.Id.Should().Be(b1.Id);
            top.TotalBorrow.Should().Be(2);
        }

        [Fact]
        public async Task GetCategoryReportAsync_WithData_AggregatesPerCategory()
        {
            // Arrange
            var c1 = new Category { Id = Guid.NewGuid(), Name = "C1" };
            var c2 = new Category { Id = Guid.NewGuid(), Name = "C2" };
            _categoryRepo.Setup(r => r.GetQueryable())
                         .Returns(new List<Category> { c1, c2 }.AsQueryable().BuildMock());

            var b1 = new Book { Id = Guid.NewGuid(), CategoryId = c1.Id, Title = "B1", Quantity = 2, Available = 1 };
            var b2 = new Book { Id = Guid.NewGuid(), CategoryId = c1.Id, Title = "B2", Quantity = 3, Available = 2 };
            var b3 = new Book { Id = Guid.NewGuid(), CategoryId = c2.Id, Title = "B3", Quantity = 1, Available = 1 };
            _bookRepo.Setup(r => r.GetQueryable())
                     .Returns(new List<Book> { b1, b2, b3 }.AsQueryable().BuildMock());

            var today = DateOnly.FromDateTime(DateTime.Today);
            var details = new List<BookBorrowingRequestDetail>
            {
                new BookBorrowingRequestDetail { BookId = b1.Id, BookBorrowingRequest = new BookBorrowingRequest { DateRequested = today } },
                new BookBorrowingRequestDetail { BookId = b2.Id, BookBorrowingRequest = new BookBorrowingRequest { DateRequested = today } }
            }.AsQueryable().BuildMock();
            _detailRepo.Setup(r => r.GetQueryable()).Returns(details);

            // Act
            var result = await _service.GetCategoryReportAsync(
                new CategoryReportQueryParameters { FromDate = today.AddDays(-1), ToDate = today.AddDays(1), Top = 2 }
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            var list = result.Data.Items;
            list.Should().HaveCount(2);

            var r1 = list.First(r => r.Id == c1.Id);
            r1.TotalBook.Should().Be(2);
            r1.QuantityBook.Should().Be(5);
            r1.AvailableBook.Should().Be(3);
            r1.TotalBorrowRequest.Should().Be(2);
            r1.MostRequestedBook.Should().Be("B1");

            var r2 = list.First(r => r.Id == c2.Id);
            r2.TotalBook.Should().Be(1);
            r2.TotalBorrowRequest.Should().Be(0);
            r2.MostRequestedBook.Should().BeNull();
        }

        [Fact]
        public async Task GetUserReportAsync_WithData_ExcludesAdminAndAppliesAudit()
        {
            // Arrange
            var adminRole = new Role { Name = RoleName.Admin.ToString() };
            var userRole = new Role { Name = RoleName.User.ToString() };
            var u1 = new User { Id = Guid.NewGuid(), Username = "u1", Email = "e1", Role = userRole, BookReviews = new List<BookReview> { new BookReview() } };
            var u2 = new User { Id = Guid.NewGuid(), Username = "u2", Email = "e2", Role = adminRole, BookReviews = null };
            _userRepo.Setup(r => r.GetQueryable())
                     .Returns(new List<User> { u1, u2 }.AsQueryable().BuildMock());

            var today = DateOnly.FromDateTime(DateTime.Today);
            _borrowReqRepo.Setup(r => r.GetQueryable())
                         .Returns(new List<BookBorrowingRequest>
                         {
                             new BookBorrowingRequest { RequesterId = u1.Id, DateRequested = today, Status = (int)BookBorrowingStatus.Approved },
                             new BookBorrowingRequest { RequesterId = u1.Id, DateRequested = today, Status = (int)BookBorrowingStatus.Rejected }
                         }.AsQueryable().BuildMock());

            var auditList = new List<UserActiveDaysAuditLog>
            {
                new UserActiveDaysAuditLog { UserId = u1.Id, ActiveDays = 7 }
            };
            _auditLogger.Setup(a => a.GetUserActivitiesReportAsync(It.IsAny<UserActivitiesQueryParameters>()))
                        .ReturnsAsync(auditList);

            // Act
            var result = await _service.GetUserReportAsync(
                new UserEngagementReportQueryParameters { FromDate = today.AddDays(-1), ToDate = today.AddDays(1), Top = 5 }
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            var resultData = result.Data;
            resultData.Should().NotBeNull();
            var list = result.Data?.Items;
            list.Should().HaveCount(1); 

            var r = list.First();
            r.Id.Should().Be(u1.Id);
            r.NumberOfBookReview.Should().Be(1);
            r.ApprovedBookBorrowingRequest.Should().Be(1);
            r.RejectedBookBorrowingRequest.Should().Be(1);
            r.TotalBookBorrowingRequest.Should().Be(2);
            r.ActiveDay.Should().Be(7);
        }
    }
}
