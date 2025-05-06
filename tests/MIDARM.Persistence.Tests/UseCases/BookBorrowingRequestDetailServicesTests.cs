using FluentAssertions;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Services;
using MockQueryable;
using Moq;

namespace MIDASM.Persistence.Tests.Services.Tests
{
    public class BookBorrowingRequestDetailServicesTests
    {
        private readonly Mock<IBookBorrowingRequestDetailRepository> _repoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly Mock<IExecutionContext> _executionContextMock;
        private readonly Mock<IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>>> _queueMock;
        private readonly IBookBorrowingRequestDetailServices _service;

        public BookBorrowingRequestDetailServicesTests()
        {
            _repoMock = new Mock<IBookBorrowingRequestDetailRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _executionContextMock = new Mock<IExecutionContext>();
            _queueMock = new Mock<IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>>>();

            _service = new BookBorrowingRequestDetailServices(
                _repoMock.Object,
                _userRepoMock.Object,
                _auditLoggerMock.Object,
                _executionContextMock.Object,
                _queueMock.Object
            );
        }

        [Fact]
        public async Task AdjustExtendDueDateAsync_WhenNotFound_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id, "BookBorrowingRequest", "Book"))
                .ReturnsAsync((BookBorrowingRequestDetail)null);

            // Act
            var result = await _service.AdjustExtendDueDateAsync(id, status: 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookBorrowingRequestDetailErrors.BookBorrowedDetailNotFound);
        }

        [Fact]
        public async Task AdjustExtendDueDateAsync_WhenRequestRejected_ReturnsFailure()
        {
            // Arrange
            var detail = new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                BookBorrowingRequest = new BookBorrowingRequest { Status = (int)BookBorrowingStatus.Rejected }
            };
            _repoMock
                .Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book"))
                .ReturnsAsync(detail);

            // Act
            var result = await _service.AdjustExtendDueDateAsync(detail.Id, status: 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookBorrowingRequestDetailErrors.BookBorrowReject);
        }

        [Fact]
        public async Task AdjustExtendDueDateAsync_WhenNoExtendDateSet_ReturnsFailure()
        {
            // Arrange
            var detail = new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                BookBorrowingRequest = new BookBorrowingRequest { Status = (int)BookBorrowingStatus.Approved },
                ExtendDueDate = null
            };
            _repoMock
                .Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book"))
                .ReturnsAsync(detail);

            // Act
            var result = await _service.AdjustExtendDueDateAsync(detail.Id, status: 0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookBorrowingRequestDetailErrors.BookBorrowedExtendDueDateInvalid);
        }

        [Fact]
        public async Task AdjustExtendDueDateAsync_WhenApproved_UpdatesDetail_QueuesMail_LogsAudit()
        {
            // Arrange
            var oldDueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var detail = new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                DueDate = oldDueDate,
                ExtendDueDate = oldDueDate.AddDays(7),
                ExtendDueDateTimes = 2,
                BookBorrowingRequest = new BookBorrowingRequest
                {
                    Status = (int)BookBorrowingStatus.Approved,
                    RequesterId = Guid.NewGuid(),
                    Requester = new User { FirstName = "John", LastName = "Doe", Email = "john@example.com" }
                },
                Book = new Book { Title = "C# in Depth", Id = Guid.NewGuid() }
            };
            _repoMock
                .Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book"))
                .ReturnsAsync(detail);

            _userRepoMock
                .Setup(u => u.GetByIdAsync(detail.BookBorrowingRequest.RequesterId))
                .ReturnsAsync(detail.BookBorrowingRequest.Requester);

            _executionContextMock
                .Setup(e => e.GetUserName())
                .Returns("tester");

            _auditLoggerMock
                .Setup(e => e.LogAsync(
                    detail.Id.ToString(),
                    nameof(BookBorrowingRequestDetail),
                    It.Is<string>(msg => msg.Contains("Book extend due date request")),
                    It.IsAny<Dictionary<string, (string?, string?)>?>()
                ))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AdjustExtendDueDateAsync(detail.Id, status: 1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(BookBorrowingRequestDetailCommandMessages.AdjustExtendDueDateRequestSuccess);

            _repoMock.Verify(r => r.Update(It.Is<BookBorrowingRequestDetail>(d => d.Id == detail.Id && d.ExtendDueDate == null)), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _queueMock.Verify(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>(), It.IsAny<CancellationToken>()), Times.Once);

            _auditLoggerMock.Verify(a => a.LogAsync(
                detail.Id.ToString(),
                nameof(BookBorrowingRequestDetail),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, (string?, string?)>?>()),
                Times.Once);
        }

        [Fact]
        public async Task GetsAsync_WithApprovedStatus_ReturnsPagedResults()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var bookId = Guid.NewGuid();
            var detail = new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                DueDate = DateOnly.FromDateTime(now),
                ExtendDueDate = DateOnly.FromDateTime(now).AddDays(5),
                ExtendDueDateTimes = 1,
                Noted = "Note",
                BookBorrowingRequestId = Guid.NewGuid(),
                BookBorrowingRequest = new BookBorrowingRequest
                {
                    Status = (int)BookBorrowingStatus.Approved,
                    Requester = new User { FirstName = "John", LastName = "Doe" },
                    Approver = new User { FirstName = "Jane", LastName = "Smith" }
                },
                BookId = bookId,
                Book = new Book
                {
                    Id = bookId,
                    Title = "Test Book",
                    Author = "Author",
                    Category = new Category { Id = Guid.NewGuid(), Name = "Cat" }
                }
            };
            var list = new List<BookBorrowingRequestDetail> { detail };
            var mockQueryable = list.AsQueryable().BuildMock();
            _repoMock.Setup(r => r.GetQueryable()).Returns(mockQueryable);

            var queryParams = new QueryParameters { PageSize = 10, PageIndex = 1 };

            // Act
            var result = await _service.GetsAsync(queryParams);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var page = result.Data;
            page.Should().NotBeNull();
            page.PageSize.Should().Be(10);
            page.PageIndex.Should().Be(1);
            page.TotalCount.Should().Be(1);
            page.Items.Should().HaveCount(1);

            var resp = page.Items.First();
            resp.Id.Should().Be(detail.Id);
            resp.BookBorrowingRequestId.Should().Be(detail.BookBorrowingRequestId);
            resp.BookId.Should().Be(detail.BookId);
            resp.DueDate.Should().Be(detail.DueDate);
            resp.Noted.Should().Be(detail.Noted);
            resp.ExtendDueDateTimes.Should().Be(detail.ExtendDueDateTimes);
            resp.ExtendDueDate.Should().Be(detail.ExtendDueDate);
            resp.RequesterName.Should().Be("JohnDoe");
            resp.ApproverName.Should().Be("JaneSmith");
            resp.Book.Id.Should().Be(detail.Book.Id);
            resp.Book.Title.Should().Be(detail.Book.Title);
            resp.Book.Author.Should().Be(detail.Book.Author);
            resp.Book.Category.Id.Should().Be(detail.Book.Category.Id);
            resp.Book.Category.Name.Should().Be(detail.Book.Category.Name);
        }

        [Fact]
        public async Task AdjustExtendDueDateAsync_WhenRejected_Status0_ProcessesAndLogs()
        {
            // Arrange
            var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var detail = new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                DueDate = dueDate,
                ExtendDueDate = dueDate.AddDays(3),
                ExtendDueDateTimes = 1,
                BookBorrowingRequest = new BookBorrowingRequest
                {
                    Status = (int)BookBorrowingStatus.Approved,
                    RequesterId = Guid.NewGuid(),
                    Requester = new User { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" }
                },
                Book = new Book { Title = "Clean Code", Id = Guid.NewGuid() }
            };
            _repoMock
                .Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book"))
                .ReturnsAsync(detail);
            _auditLoggerMock
                .Setup(e => e.LogAsync(
                    detail.Id.ToString(),
                    nameof(BookBorrowingRequestDetail),
                    It.Is<string>(msg => msg.Contains("Book extend due date request")),
                    It.IsAny<Dictionary<string, (string?, string?)>>()
                ))
                .Returns(Task.CompletedTask);
            _userRepoMock
                .Setup(u => u.GetByIdAsync(detail.BookBorrowingRequest.RequesterId))
                .ReturnsAsync(detail.BookBorrowingRequest.Requester);

            _executionContextMock
                .Setup(e => e.GetUserName())
                .Returns("alice");

            // Act
            var result = await _service.AdjustExtendDueDateAsync(detail.Id, status: 0);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(BookBorrowingRequestDetailCommandMessages.AdjustExtendDueDateRequestSuccess);

            _repoMock.Verify(r => r.Update(It.Is<BookBorrowingRequestDetail>(d => d.Id == detail.Id && d.ExtendDueDate == null)), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _queueMock.Verify(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>(), It.IsAny<CancellationToken>()), Times.Once);
            _auditLoggerMock.Verify(a => a.LogAsync(
                detail.Id.ToString(),
                nameof(BookBorrowingRequestDetail),
                 It.IsAny<string>(),
                It.IsAny<Dictionary<string, (string?, string?)>>()),
                Times.Once);
        }
    }
}
