using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.UseCases;
using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Contract.SharedKernel;
using MIDASM.Application.Commons.Models.Users;
using MISARM.API.Tests.TestHelpers;

namespace MIDASM.API.Tests.Controllers
{
    public class BookBorrowingRequestsControllerTests
    {
        private readonly Mock<IBookBorrowingRequestServices> _serviceMock;
        private readonly BookBorrowingRequestsController _controller;

        public BookBorrowingRequestsControllerTests()
        {
            _serviceMock = new Mock<IBookBorrowingRequestServices>();
            _controller = new BookBorrowingRequestsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetsAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new BookBorrowingRequestQueryParameters();

            var fakeResult = ResultPaginationCreateHelper.CreateStubResult<BookBorrowingRequestData>();
            _serviceMock
                .Setup(s => s.GetsAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetsAsync(query);

            // Assert
            _serviceMock.Verify(s => s.GetsAsync(query), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new BookBorrowingStatusUpdateRequest {};
            var fakeResult = Result<string>.Success("status-changed");
            _serviceMock
                .Setup(s => s.ChangeStatusAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.UpdateStatusAsync(request);

            // Assert
            _serviceMock.Verify(s => s.ChangeStatusAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetDetailByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detail = new BookBorrowingRequestDetailResponse();
            var fakeResult = Result<BookBorrowingRequestDetailResponse>.Success(detail);
            _serviceMock
                .Setup(s => s.GetDetailAsync(id))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetDetailByIdAsync(id);

            // Assert
            _serviceMock.Verify(s => s.GetDetailAsync(id), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }
    }
}
