using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using Moq;

namespace MIDASM.API.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryServices> _categoryServicesMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _categoryServicesMock = new Mock<ICategoryServices>();
            _controller = new CategoriesController(_categoryServicesMock.Object);
        }

        [Fact]
        public async Task GetAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new CategoriesQueryParameters();
            Result<PaginationResult<CategoryResponse>> fakeResult = PaginationResult<CategoryResponse>.Create(10, new List<CategoryResponse>());
            _categoryServicesMock
                .Setup(s => s.GetCategoriesAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetAsync(query);

            // Assert
            _categoryServicesMock.Verify(s => s.GetCategoriesAsync(query), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetNameAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            Result<PaginationResult<CategoryResponse>> fakeResult = PaginationResult<CategoryResponse>.Create(10, new List<CategoryResponse>());
            _categoryServicesMock
                .Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetNameAsync();

            // Assert
            _categoryServicesMock.Verify(s => s.GetCategoriesAsync(), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fakeResult = Result<CategoryResponse>.Success(new CategoryResponse());
            _categoryServicesMock
                .Setup(s => s.GetCategoryByIdAsync(id))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetByIdAsync(id);

            // Assert
            _categoryServicesMock.Verify(s => s.GetCategoryByIdAsync(id), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new CategoryCreateRequest();
            Result<string> fakeResult = "Success";
            _categoryServicesMock
                .Setup(s => s.CreateCategoryAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.CreateAsync(request);

            // Assert
            _categoryServicesMock.Verify(s => s.CreateCategoryAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new CategoryUpdateRequest();
            Result<string> fakeResult = "Success";
            _categoryServicesMock
                .Setup(s => s.UpdateCategoryAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.UpdateAsync(request);

            // Assert
            _categoryServicesMock.Verify(s => s.UpdateCategoryAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            Result<string> fakeResult = "Success";
            _categoryServicesMock
                .Setup(s => s.DeleteCategoryAsync(id))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.DeleteAsync(id);

            // Assert
            _categoryServicesMock.Verify(s => s.DeleteCategoryAsync(id), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }
    }
}
