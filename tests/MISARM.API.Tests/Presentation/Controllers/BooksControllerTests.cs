
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.UseCases;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Contract.SharedKernel;


namespace MIDASM.API.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookServices> _bookServicesMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _bookServicesMock = new Mock<IBookServices>();
            _controller = new BooksController(_bookServicesMock.Object);
        }

        [Fact]
        public async Task GetsAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new BooksQueryParameters();
            Result<PaginationResult<BookResponse>> fakeResult = PaginationResult<BookResponse>.Create(10, new());
            _bookServicesMock
                .Setup(s => s.GetsAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetsAsync(query);

            // Assert
            _bookServicesMock.Verify(s => s.GetsAsync(query), Times.Once);
            actionResult
                .Should().BeOfType<OkObjectResult>()           
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fakeResult = Result<BookDetailResponse>.Success(new());
            _bookServicesMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetByIdAsync(id);

            // Assert
            _bookServicesMock.Verify(s => s.GetByIdAsync(id), Times.Once);
            actionResult
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new BookCreateRequest();

            Result<string> fakeResult = "Success";
            _bookServicesMock
                .Setup(s => s.CreateAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.CreateAsync(request);

            // Assert
            _bookServicesMock.Verify(s => s.CreateAsync(request), Times.Once);
            actionResult
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new BookUpdateRequest();
            Result<string> fakeResult = "Success";
            _bookServicesMock
                .Setup(s => s.UpdateAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.UpdateAsync(request);

            // Assert
            _bookServicesMock.Verify(s => s.UpdateAsync(request), Times.Once);
            actionResult
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = Guid.NewGuid();
            Result<string> fakeResult = "Success";
            _bookServicesMock
                .Setup(s => s.DeleteAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.DeleteAsync(request);

            // Assert
            _bookServicesMock.Verify(s => s.DeleteAsync(request), Times.Once);
            actionResult
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

    }
}
