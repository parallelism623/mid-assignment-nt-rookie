using FluentAssertions;
using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Services.Tests
{
    public class CategoryServicesTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<ITransactionManager> _transMgrMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly Mock<IExecutionContext> _executionContextMock;
        private readonly CategoryServices _service;

        public CategoryServicesTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _bookRepoMock = new Mock<IBookRepository>();
            _transMgrMock = new Mock<ITransactionManager>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _executionContextMock = new Mock<IExecutionContext>();

            _service = new CategoryServices(
                _categoryRepoMock.Object,
                _bookRepoMock.Object,
                _transMgrMock.Object,
                _auditLoggerMock.Object,
                _executionContextMock.Object
            );
        }

        [Fact]
        public async Task GetCategoriesAsync_WithPaging_ReturnsPagedResults()
        {
            // Arrange
            var cats = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Cat1", Description = "D1" },
                new Category { Id = Guid.NewGuid(), Name = "Cat2", Description = "D2" }
            }.AsQueryable();
            var mockCats = cats.BuildMock();
            _categoryRepoMock.Setup(r => r.GetQueryable()).Returns(mockCats);

            var parms = new CategoriesQueryParameters { PageIndex = 1, PageSize = 10 };

            // Act
            var result = await _service.GetCategoriesAsync(parms);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var page = result.Data;
            page.Should().NotBeNull();  
            page.PageIndex.Should().Be(1);
            page.PageSize.Should().Be(10);
            page.TotalCount.Should().Be(2);
            page.Items.Should().HaveCount(2);
            page.Items.First().Id.Should().Be(cats.ElementAt(0).Id);
            page.Items.First().Name.Should().Be("Cat1");
        }

        [Fact]
        public async Task GetCategoriesAsync_NoParams_ReturnsAllCategories()
        {
            // Arrange
            var cats = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "C1", Description = "D1" }
            }.AsQueryable().BuildMock();
            _categoryRepoMock.Setup(r => r.GetQueryable()).Returns(cats);

            // Act
            var result = await _service.GetCategoriesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            var page = result.Data;
            page.TotalCount.Should().Be(1);
            page.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WhenExists_ReturnsResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cat = new Category { Id = id, Name = "X", Description = "D" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(id))
                             .ReturnsAsync(cat);

            // Act
            var result = await _service.GetCategoryByIdAsync(id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(id);
            result.Data.Name.Should().Be("X");
        }

        [Fact]
        public async Task CreateCategoryAsync_NameExists_ReturnsFailure()
        {
            // Arrange
            var req = new CategoryCreateRequest { Name = "Name1", Description = "Desc" };
            _categoryRepoMock.Setup(r => r.GetByNameAsync("Name1")).ReturnsAsync(new Category());

            // Act
            var result = await _service.CreateCategoryAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(CategoryErrors.CategoryNameExists);
        }

        [Fact]
        public async Task CreateCategoryAsync_Valid_AddsAndLogsAndReturnsSuccess()
        {
            // Arrange
            var req = new CategoryCreateRequest { Name = "New", Description = "Desc" };
            _categoryRepoMock.Setup(r => r.GetByNameAsync("New")).ReturnsAsync((Category)null);
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _auditLoggerMock.Setup(a => a.LogAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()
            )).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCategoryAsync(req);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(CategoryCommandMessages.CategoryCreatedSuccess);
            _categoryRepoMock.Verify(r => r.Add(It.Is<Category>(c => c.Name == "New")), Times.Once);
            _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _auditLoggerMock.Verify(a => a.LogAsync(
                It.IsAny<string>(), nameof(Category), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()
            ), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_NotFound_ReturnsFailure()
        {
            // Arrange
            var req = new CategoryUpdateRequest { Id = Guid.NewGuid(), Name = "N", Description = "D" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync((Category)null);

            // Act
            var result = await _service.UpdateCategoryAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(CategoryErrors.CategoryNotFound);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Valid_UpdatesAndLogsAndReturnsSuccess()
        {
            // Arrange
            var cat = new Category { Id = Guid.NewGuid(), Name = "Old", Description = "D" };
            var req = new CategoryUpdateRequest { Id = cat.Id, Name = "New", Description = "Desc" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(cat.Id)).ReturnsAsync(cat);
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _auditLoggerMock.Setup(a => a.LogAsync(
                It.IsAny<string>(), nameof(Category), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>())
            ).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateCategoryAsync(req);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(CategoryCommandMessages.CategoryUpdatedSuccess);
            _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _auditLoggerMock.Verify(a => a.LogAsync(
                cat.Id.ToString(), nameof(Category), It.Is<string>(msg => msg.Contains("update")), It.IsAny<Dictionary<string, (string?, string?)>>()
            ), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_NotFound_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(id))
                             .ReturnsAsync((Category)null);

            // Act
            var result = await _service.DeleteCategoryAsync(id);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(CategoryErrors.CategoryNotFound);
        }

        [Fact]
        public async Task DeleteCategoryAsync_HasBooks_ReturnsFailure()
        {
            // Arrange
            var cat = new Category { Id = Guid.NewGuid(), Books = new List<Book> { new Book { Quantity = 1, Available = 0 } } };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(cat.Id, It.IsAny<Expression<Func<Category, object>>>()))
                             .ReturnsAsync(cat);

            // Act
            var result = await _service.DeleteCategoryAsync(cat.Id);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain((CategoryErrors.CategoryCanNotDelete));
        }

        [Fact]
        public async Task DeleteCategoryAsync_Valid_DeletesAndLogsAndReturnsSuccess()
        {
            // Arrange
            var cat = new Category { Id = Guid.NewGuid(), Name = "Cat", Books = new List<Book>() };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(cat.Id, It.IsAny<Expression<Func<Category, object>>>()))
                             .ReturnsAsync(cat);
            _bookRepoMock.Setup(r => r.GetQueryable()).Returns(new List<Book>().AsQueryable().BuildMock());
            _bookRepoMock.Setup(r => r.UpdateRange(It.IsAny<IEnumerable<Book>>()));
            _bookRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _transMgrMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _transMgrMock.Setup(t => t.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _transMgrMock.Setup(t => t.DisposeTransaction());
            _auditLoggerMock.Setup(a => a.LogAsync(
                It.IsAny<string>(), nameof(Category), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>())
            ).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteCategoryAsync(cat.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(CategoryCommandMessages.CategoryDeletedSuccess);
            _transMgrMock.Verify(t => t.BeginTransactionAsync(), Times.Once);
            _bookRepoMock.Verify(b => b.UpdateRange(It.IsAny<IEnumerable<Book>>()), Times.Once);
            _bookRepoMock.Verify(b => b.SaveChangesAsync(), Times.Once);
            _categoryRepoMock.Verify(r => r.Update(It.Is<Category>(c => c.IsDeleted)), Times.Once);
            _transMgrMock.Verify(t => t.CommitTransactionAsync(), Times.Once);
            _transMgrMock.Verify(t => t.DisposeTransaction(), Times.Once);
            _auditLoggerMock.Verify(a => a.LogAsync(
                cat.Id.ToString(), nameof(Category), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_OnException_RollsBackAndReturnsFailure()
        {
            // Arrange
            var cat = new Category { Id = Guid.NewGuid(), Books = new List<Book>() };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(cat.Id, It.IsAny<Expression<Func<Category, object>>>())).ReturnsAsync(cat);
            _transMgrMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _bookRepoMock.Setup(b => b.UpdateRange(It.IsAny<IEnumerable<Book>>()));
            _categoryRepoMock
                .Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new Exception("fail"));
            _transMgrMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);
            _transMgrMock.Setup(t => t.DisposeTransaction());

            // Act
            var result = await _service.DeleteCategoryAsync(cat.Id);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(CategoryErrors.CategoryDeleteFail);
            _transMgrMock.Verify(t => t.RollbackAsync(), Times.Once);
            _transMgrMock.Verify(t => t.DisposeTransaction(), Times.Once);
        }
    }
}
