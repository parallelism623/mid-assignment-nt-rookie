using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using MISARM.API.Tests.TestHelpers;
using Moq;

namespace MIDASM.API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserServices> _userServicesMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServicesMock = new Mock<IUserServices>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _controller = new UsersController(_userServicesMock.Object, _auditLoggerMock.Object);
        }

        [Fact]
        public async Task CreateBookBorrowingRequestAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var request = new BookBorrowingRequestCreate {};
            var fakeResult = Result<string>.Success("created");
            _userServicesMock
                .Setup(s => s.CreateBookBorrowingRequestAsync(request))
                .ReturnsAsync(fakeResult);

            var result = await _controller.CreateBookBorrowingRequestAsync(request);

            _userServicesMock.Verify(s => s.CreateBookBorrowingRequestAsync(request), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetBookBorrowingRequestByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var query = new UserBookBorrowingRequestQueryParameters();
            var fakeResult = ResultPaginationCreateHelper.CreateStubResult< BookBorrowingRequestResponse>();
            _userServicesMock
                .Setup(s => s.GetBookBorrowingRequestByIdAsync(id, query))
                .ReturnsAsync(fakeResult);

            var result = await _controller.GetBookBorrowingRequestByIdAsync(id, query);

            _userServicesMock.Verify(s => s.GetBookBorrowingRequestByIdAsync(id, query), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetBookBorrowedRequestDetailByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var query = new QueryParameters();
            var fakeResult = ResultPaginationCreateHelper.CreateStubResult<BookBorrowedRequestDetailResponse>();
            _userServicesMock
                .Setup(s => s.GetBookBorrowedRequestDetailByIdAsync(id, query))
                .ReturnsAsync(fakeResult);

            var result = await _controller.GetBookBorrowedRequestDetailByIdAsync(id, query);

            _userServicesMock.Verify(s => s.GetBookBorrowedRequestDetailByIdAsync(id, query), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task ExtendDueDateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var request = new DueDatedExtendRequest {};
            var fakeResult = Result<string>.Success("extended");
            _userServicesMock
                .Setup(s => s.ExtendDueDateBookBorrowed(request))
                .ReturnsAsync(fakeResult);

            var result = await _controller.ExtendDueDateAsync(id, request);

            _userServicesMock.Verify(s => s.ExtendDueDateBookBorrowed(request), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var user = new UserDetailResponse();
            var fakeResult = Result<UserDetailResponse>.Success(user);
            _userServicesMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(fakeResult);

            var result = await _controller.GetByIdAsync(id);

            _userServicesMock.Verify(s => s.GetByIdAsync(id), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var query = new UserQueryParameters();
            var fakeResult = ResultPaginationCreateHelper.CreateStubResult<UserDetailResponse>();
            _userServicesMock
                .Setup(s => s.GetAsync(query))
                .ReturnsAsync(fakeResult);

            var result = await _controller.GetAsync(query);

            _userServicesMock.Verify(s => s.GetAsync(query), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var request = new UserCreateRequest {  };
            var fakeResult = Result<string>.Success("created");
            _userServicesMock
                .Setup(s => s.CreateAsync(request))
                .ReturnsAsync(fakeResult);

            var result = await _controller.CreateAsync(request);

            _userServicesMock.Verify(s => s.CreateAsync(request), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var request = new UserUpdateRequest {  };
            var fakeResult = Result<string>.Success("updated");
            _userServicesMock
                .Setup(s => s.UpdateAsync(request))
                .ReturnsAsync(fakeResult);

            var result = await _controller.UpdateAsync(request);

            _userServicesMock.Verify(s => s.UpdateAsync(request), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var fakeResult = Result<string>.Success("deleted");
            _userServicesMock
                .Setup(s => s.DeleteAsync(id))
                .ReturnsAsync(fakeResult);

            var result = await _controller.DeleteAsync(id);

            _userServicesMock.Verify(s => s.DeleteAsync(id), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetAuditLogsAsync_WhenCalled_InvokesLoggerAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var query = new UserAuditLogQueryParameters();
            var fakeResult = ResultPaginationCreateHelper.CreateStubResult< UserAuditLogResponse>();
            _auditLoggerMock
                .Setup(l => l.GetUserActivitiesAsync(id, query))
                .ReturnsAsync(fakeResult);

            var result = await _controller.GetAuditLogsAsync(id, query);

            _auditLoggerMock.Verify(l => l.GetUserActivitiesAsync(id, query), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            var id = Guid.NewGuid();
            var request = new UserProfileUpdateRequest {  };
            var fakeResult = Result<string>.Success("profile-updated");
            _userServicesMock
                .Setup(s => s.UpdateProfileAsync(id, request))
                .ReturnsAsync(fakeResult);

            var result = await _controller.UpdateProfileAsync(id, request);

            _userServicesMock.Verify(s => s.UpdateProfileAsync(id, request), Times.Once);
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(fakeResult);
        }
    }
}
