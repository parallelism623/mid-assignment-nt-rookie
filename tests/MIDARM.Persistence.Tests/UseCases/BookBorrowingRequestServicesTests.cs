using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using MIDASM.Application.Commons.Errors;
using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.UseCases.Implements;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MockQueryable;
using Moq;

namespace MIDASM.Persistence.Services.Tests
{
    public class BookBorrowingRequestServicesTests
    {
        private readonly Mock<IBookBorrowingRequestRepository> _repoMock;
        private readonly Mock<IExecutionContext> _executionContextMock;
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>>> _queueMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly BookBorrowingRequestServices _service;

        public BookBorrowingRequestServicesTests()
        {
            _repoMock = new Mock<IBookBorrowingRequestRepository>();
            _executionContextMock = new Mock<IExecutionContext>();
            _bookRepoMock = new Mock<IBookRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _queueMock = new Mock<IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>>>();
            _auditLoggerMock = new Mock<IAuditLogger>();

            _service = new BookBorrowingRequestServices(
                _repoMock.Object,
                _executionContextMock.Object,
                _bookRepoMock.Object,
                _userRepoMock.Object,
                _queueMock.Object,
                _auditLoggerMock.Object
            );
        }

        [Fact]
        public async Task GetsAsync_ReturnsPagedData_MappingCorrectly()
        {
            // Arrange
            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            var entity = new BookBorrowingRequest
            {
                Id = Guid.NewGuid(),
                DateRequested = now,
                Status = (int)BookBorrowingStatus.Approved,
                Requester = new User { FirstName = "John", LastName = "Doe", Username = "jdoe" },
            };
            var list = new List<BookBorrowingRequest> { entity };
            var mockQ = list.AsQueryable().BuildMock();
            _repoMock.Setup(r => r.GetQueryable()).Returns(mockQ);

            var queryParams = new BookBorrowingRequestQueryParameters { PageSize = 5, PageIndex = 1 };

            // Act
            var result = await _service.GetsAsync(queryParams);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var page = result.Data;
            page.Should().NotBeNull();
            page.PageSize.Should().Be(5);
            page.PageIndex.Should().Be(1);
            page.TotalCount.Should().Be(1);
            page.Items.Should().HaveCount(1);
            var data = page.Items.First();
            data.Id.Should().Be(entity.Id);
            data.DateRequested.Should().Be(now);
            data.Status.Should().Be((int)BookBorrowingStatus.Approved);
            data.Requester.FirstName.Should().Be("John");
            data.Requester.LastName.Should().Be("Doe");
        }

        [Fact]
        public async Task ChangeStatusAsync_NotFound_ReturnsFailure()
        {
            // Arrange
            var req = new BookBorrowingStatusUpdateRequest { Id = Guid.NewGuid(), Status = (int)BookBorrowingStatus.Approved };
            _repoMock.Setup(r => r.GetByIdAsync(req.Id, It.IsAny<string[]>())).ReturnsAsync((BookBorrowingRequest)null);

            // Act
            var result = await _service.ChangeStatusAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookBorrowingRequestErrors.NotFound);
        }

        [Fact]
        public async Task ChangeStatusAsync_StatusNotWaiting_ReturnsFailure()
        {
            // Arrange
            var entity = new BookBorrowingRequest { Id = Guid.NewGuid(), Status = (int)BookBorrowingStatus.Approved };
            _repoMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<string[]>())).ReturnsAsync(entity);
            var req = new BookBorrowingStatusUpdateRequest { Id = entity.Id, Status = (int)BookBorrowingStatus.Approved };

            // Act
            var result = await _service.ChangeStatusAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookBorrowingRequestErrors.CanNotUpdateCurrentStatus);
        }

        [Fact]
        public async Task ChangeStatusAsync_Reject_IncrementsBooks_SendsMail_LogsAudit()
        {
            // Arrange
            var user = new User {Id = Guid.NewGuid(), FirstName = "A", LastName = "B", Email = "a@b.com", Username = "ab" };
            var entity = new BookBorrowingRequest
            {
                Id = Guid.NewGuid(),
                Status = (int)BookBorrowingStatus.Waiting,
                RequesterId = Guid.NewGuid(),
                Requester = user,
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
                {
                    new BookBorrowingRequestDetail{ BookId = Guid.NewGuid() }
                }
            };
            _repoMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<string[]>())).ReturnsAsync(entity);
            _bookRepoMock.Setup(b => b.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Book> { new Book { Available = 0 } });
            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            var req = new BookBorrowingStatusUpdateRequest { Id = entity.Id, Status = (int)BookBorrowingStatus.Rejected };
            _executionContextMock.Setup(e => e.GetUserName()).Returns("tester");

            // Act
            var result = await _service.ChangeStatusAsync(req);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(BookBorrowingRequestCommandMessages.ChangeStatusSuccess);

            _bookRepoMock.Verify(b => b.UpdateRange(It.IsAny<IEnumerable<Book>>()), Times.Once);
            _bookRepoMock.Verify(b => b.SaveChangesAsync(), Times.Once);
            _queueMock.Verify(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>(), It.IsAny<CancellationToken>()), Times.Once);

            _auditLoggerMock.Verify(a => a.LogAsync(
                entity.Id.ToString(), nameof(BookBorrowingRequest), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()),
                Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_Approve_SendsMail_LogsAudit()
        {
            // Arrange
            var user = new User {Id = Guid.NewGuid(), FirstName = "X", LastName = "Y", Email = "x@y.com", Username = "xy" };
            var entity = new BookBorrowingRequest
            {
                Id = Guid.NewGuid(),
                Status = (int)BookBorrowingStatus.Waiting,
                RequesterId = Guid.NewGuid(),
                Requester = user
            };
            _repoMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<string[]>())).ReturnsAsync(entity);
            var req = new BookBorrowingStatusUpdateRequest { Id = entity.Id, Status = (int)BookBorrowingStatus.Approved };
            _executionContextMock.Setup(e => e.GetUserName()).Returns("tester");
            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            // Act
            var result = await _service.ChangeStatusAsync(req);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(BookBorrowingRequestCommandMessages.ChangeStatusSuccess);

            _queueMock.Verify(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>(), It.IsAny<CancellationToken>()), Times.Once);

            _auditLoggerMock.Verify(a => a.LogAsync(
                entity.Id.ToString(), nameof(BookBorrowingRequest), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetDetailAsync_WhenFound_ReturnsDetailResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new BookBorrowingRequest { Id = id };
            _repoMock.Setup(r => r.GetDetailAsync(id)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetDetailAsync(id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(id);
        }

    }
}
