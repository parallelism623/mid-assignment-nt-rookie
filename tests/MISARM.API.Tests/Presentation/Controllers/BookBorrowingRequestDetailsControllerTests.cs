using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;


using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.UseCases;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.API.Tests.Controllers
{
    public class BookBorrowingRequestDetailsControllerTests
    {
        private readonly Mock<IBookBorrowingRequestDetailServices> _serviceMock;
        private readonly BookBorrowingRequestDetailsController _controller;

        public BookBorrowingRequestDetailsControllerTests()
        {
            _serviceMock = new Mock<IBookBorrowingRequestDetailServices>();
            _controller = new BookBorrowingRequestDetailsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetsAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new QueryParameters {};
            Result<PaginationResult<BookBorrowedRequestDetailResponse>> fakeResult = PaginationResult<BookBorrowedRequestDetailResponse>.Create(5, new());
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
        public async Task AdjustDueDateExtendAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new BookBorrowedExtendDueDateRequest { Status = 7 };
            var fakeResult = Result<string>.Success("extended");
            _serviceMock
                .Setup(s => s.AdjustExtendDueDateAsync(id, request.Status))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.AdjustDueDateExtendAsync(id, request);

            // Assert
            _serviceMock.Verify(s => s.AdjustExtendDueDateAsync(id, request.Status), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }
    }
}
