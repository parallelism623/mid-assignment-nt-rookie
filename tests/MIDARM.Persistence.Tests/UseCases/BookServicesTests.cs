using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MIDASM.Application.Commons.Errors;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.FileServices;
using MIDASM.Application.UseCases.Implements;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MockQueryable.Moq;
using Moq;
using System.Text;

public class BookServicesTests
{
    private readonly Mock<IBookRepository> _bookRepo;
    private readonly Mock<ICategoryRepository> _categoryRepo;
    private readonly Mock<IBookReviewRepository> _bookReviewRepo;
    private readonly Mock<IImageStorageServices> _imageStorage;
    private readonly Mock<IAuditLogger> _auditLogger;
    private readonly Mock<IExecutionContext> _executionContext;
    private readonly BookServices _service;

    public BookServicesTests()
    {
        _bookRepo = new Mock<IBookRepository>();
        _categoryRepo = new Mock<ICategoryRepository>();
        _bookReviewRepo = new Mock<IBookReviewRepository>();
        _imageStorage = new Mock<IImageStorageServices>();
        _auditLogger = new Mock<IAuditLogger>();
        _executionContext = new Mock<IExecutionContext>();

        _service = new BookServices(
            _bookRepo.Object,
            _categoryRepo.Object,
            _bookReviewRepo.Object,
            _imageStorage.Object,
            _auditLogger.Object,
            _executionContext.Object
        );
    }

    [Fact]
    public async Task GetsAsync_ShouldReturnPagedBookResponses_WithCorrectReviewStats_AndSignedUrls()
    {
        // Arrange
        var b1 = new Book
        {
            Id = Guid.NewGuid(),
            Title = "T1",
            Description = "D1",
            Author = "A1",
            Quantity = 5,
            Available = 5,
            ImageUrl = null,
            Category = new Category { Id = Guid.NewGuid(), Name = "Cat1" }
        };
        var b2 = new Book
        {
            Id = Guid.NewGuid(),
            Title = "T2",
            Description = "D2",
            Author = "A2",
            Quantity = 3,
            Available = 2,
            ImageUrl = "img2.jpg",
            Category = new Category { Id = Guid.NewGuid(), Name = "Cat2" }
        };
        var books = new List<Book> { b1, b2 }.BuildMockDbSet();

        var reviews = new List<BookReview>
        {
            new(){ BookId = b1.Id, Rating = 4 },
            new(){ BookId = b1.Id, Rating = 2 },
            new(){ BookId = b2.Id, Rating = 5 }
        }.BuildMockDbSet();
        _bookRepo.Setup(x => x.GetQueryable()).Returns(books.Object.AsQueryable());
        _bookReviewRepo.Setup(x => x.GetQueryable()).Returns(reviews.Object.AsQueryable());
        _imageStorage
            .Setup(x => x.GetPreSignedUrlImage(It.IsAny<string>()))
            .ReturnsAsync((string key) => $"signed-{key}");

        var parms = new BooksQueryParameters { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await _service.GetsAsync(parms);

        result.Should().NotBeNull();

        var paginationResult = result.Data;
        paginationResult.Should().NotBeNull();
        // Assert
        paginationResult.TotalCount.Should().Be(2);
        paginationResult.Items.Should().HaveCount(2);

        var item1 = paginationResult.Items.Single(i => i.Id == b1.Id);
        item1.NumberOfReview.Should().Be(2);
        item1.AverageRating.Should().Be(3m);
        item1.ImageUrlSigned.Should().Be("signed-default.jpeg");

        var item2 = paginationResult.Items.Single(i => i.Id == b2.Id);
        item2.NumberOfReview.Should().Be(1);
        item2.AverageRating.Should().Be(5m);
        item2.ImageUrlSigned.Should().Be("signed-img2.jpg");

        _imageStorage.Verify(x => x.GetPreSignedUrlImage(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_InvalidCategory_ReturnsFailure()
    {
        // Arrange
        var req = new BookCreateRequest
        {
            Title = "T",
            Description = "D",
            Author = "A",
            Quantity = 1,
            Available = 1,
            CategoryId = Guid.NewGuid()
        };
        _categoryRepo.Setup(x => x.GetByIdAsync(req.CategoryId)).ReturnsAsync((Category)null!);

        // Act
        var result = await _service.CreateAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var firstError = result.Errors.First();
        firstError.Should().Be(BookErrors.BookCanNotCreateDueToInvalidCategory);
    }
    [Fact]
    public async Task CreateAsync_ValidRequest_UploadsFiles_AddsAndSavesAndLogs()
    {
        // Arrange
        var catId = Guid.NewGuid();

        
        var mainStream = new MemoryStream(Encoding.UTF8.GetBytes("dummy"));
        var mainImage = new FormFile(mainStream, 0, mainStream.Length, "file", "main.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        var sub1Stream = new MemoryStream(Encoding.UTF8.GetBytes("dummy1"));
        var sub1 = new FormFile(sub1Stream, 0, sub1Stream.Length, "file", "sub1.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var sub2Stream = new MemoryStream(Encoding.UTF8.GetBytes("dummy2"));
        var sub2 = new FormFile(sub2Stream, 0, sub2Stream.Length, "file", "sub2.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var req = new BookCreateRequest
        {
            Title = "T",
            Description = "D",
            Author = "A",
            Quantity = 2,
            Available = 2,
            CategoryId = catId,
            ImageUrl = mainImage,
            SubImagesUrl = new() { sub1, sub2 }
        };

        
        _categoryRepo
            .Setup(x => x.GetByIdAsync(catId))
            .ReturnsAsync(new Category { Id = catId });

       
        _imageStorage
            .Setup(x => x.UploadImageAsync(mainImage, It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploaded/main.jpg");
        _imageStorage
            .Setup(x => x.UploadImageAsync(sub1, It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploaded/sub1.png");
        _imageStorage
            .Setup(x => x.UploadImageAsync(sub2, It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploaded/sub2.png");

        // Act
        var result = await _service.CreateAsync(req);

        // Assert

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(BookCommandMessages.BookCreatedSuccess);

      
        _bookRepo.Verify(x => x.Add(It.Is<Book>(b =>
            b.Title == "T" &&
            b.ImageUrl == "uploaded/main.jpg" &&
            b.SubImagesUrl.SequenceEqual(new[]
            {
            "uploaded/sub1.png",
            "uploaded/sub2.png"
            })
        )), Times.Once);

        _bookRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        _auditLogger.Verify(x => x.LogAsync(
            It.IsAny<string>(),
            nameof(Book),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>?>()
        ), Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _bookRepo.Setup(x => x.GetByIdAsync(id, It.IsAny<string[]>()))
                 .ReturnsAsync((Book)null!);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(BookErrors.BookCanNotFound);
    }

    [Fact]
    public async Task DeleteAsync_WithBorrowRequests_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book
        {
            Id = id,
            BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
            {
                new() { BookId = id }
            }
        };
        _bookRepo.Setup(x => x.GetByIdAsync(id, It.IsAny<string[]>()))
                 .ReturnsAsync(book);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(BookErrors.BookCanNotDeletedDueToExistsBorrowRequest);
    }

    [Fact]
    public async Task DeleteAsync_Valid_RemovesAndLogs()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book { Id = id, BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>() };
        _bookRepo.Setup(x => x.GetByIdAsync(id, It.IsAny<string[]>()))
                 .ReturnsAsync(book);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.IsDeleted.Should().BeTrue();
        _bookRepo.Verify(x => x.Update(It.Is<Book>(b => b.IsDeleted)), Times.Once);
        _bookRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        _auditLogger.Verify(x => x.LogAsync(
            id.ToString(),
            nameof(Book),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>?>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetByIdsAsync_InvalidIdString_ReturnsFailure()
    {
        // Act
        var result = await _service.GetByIdsAsync("badguid,123");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(BookErrors.BookIdInvalid);
    }

    [Fact]
    public async Task GetByIdsAsync_ValidIds_ReturnsMappedList()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var books = new List<Book>
        {
            new() { Id = id1, Title = "T1", Description = "D1", Author = "A1", Quantity = 1, Available = 1 },
            new() { Id = id2, Title = "T2", Description = "D2", Author = "A2", Quantity = 2, Available = 2 }
        };
        _bookRepo.Setup(x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.SequenceEqual(new[] { id1, id2 }))))
                 .ReturnsAsync(books);

        // Act
        var result = await _service.GetByIdsAsync($"{id1},{id2}");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Select(r => r.Id).Should().BeEquivalentTo(new List<Guid> { id1, id2 });
    }

    [Fact]
    public async Task GetSignedUrlSubImage_DelegatesToImageStorage()
    {
        // Arrange
        _imageStorage.Setup(x => x.GetPreSignedUrlImage("key.png"))
                     .ReturnsAsync("signed-url");

        // Act
        var url = await _service.GetSignedUrlSubImage("key.png");

        // Assert
        url.Should().Be("signed-url");
        _imageStorage.Verify(x => x.GetPreSignedUrlImage("key.png"), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_WithSubImages_AndImageUrl_ReturnsSignedUrls()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Book
        {
            Id = id,
            Title = "Title",
            Author = "Author",
            ImageUrl = "main.jpg",
            SubImagesUrl = new List<string> { "sub1.png", "sub2.png" },
            Category = new Category { Id = Guid.NewGuid(), Name = "Cat" },
            BookReviews = new List<BookReview>()
        };
        _bookRepo
            .Setup(r => r.GetByIdAsync(id, "Category", "BookReviews"))
            .ReturnsAsync(entity);

        _imageStorage
            .Setup(m => m.GetPreSignedUrlImage(It.IsAny<string>()))
            .ReturnsAsync((string key) => $"signed-{key}");

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        _bookRepo.Verify(r => r.GetByIdAsync(id, "Category", "BookReviews"), Times.Once);
        // Signed sub-images
        _imageStorage.Verify(m => m.GetPreSignedUrlImage("sub1.png"), Times.Once);
        _imageStorage.Verify(m => m.GetPreSignedUrlImage("sub2.png"), Times.Once);
        // Signed main image
        _imageStorage.Verify(m => m.GetPreSignedUrlImage("main.jpg"), Times.Once);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Data;
        dto.Id.Should().Be(id);
        dto.SubImagesUrlSigned.Should().ContainInOrder("signed-sub1.png", "signed-sub2.png");
        dto.ImageUrlSigned.Should().Be("signed-main.jpg");
    }

    [Fact]
    public async Task GetByIdAsync_NoSubImages_AndNullImageUrl_UsesDefaultImage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Book
        {
            Id = id,
            Title = "T",
            Author = "A",
            ImageUrl = null,
            SubImagesUrl = null,
            Category = new Category { Id = Guid.NewGuid(), Name = "X" },
            BookReviews = new List<BookReview>()
        };
        _bookRepo
            .Setup(r => r.GetByIdAsync(id, It.IsAny<string[]>()))
            .ReturnsAsync(entity);

        _imageStorage
            .Setup(m => m.GetPreSignedUrlImage(It.IsAny<string>()))
            .ReturnsAsync("signed");

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        _imageStorage.Verify(m => m.GetPreSignedUrlImage(It.IsAny<string>()), Times.Once);
        var dto = result.Data;
        dto.ImageUrlSigned.Should().Be($"signed");
        dto.SubImagesUrlSigned.Should().BeNullOrEmpty();
    }
    [Fact]
    public async Task UpdateAsync_WhenNewImageProvided_DeletesOldAndUploadsNew()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Old",
            Description = "D",
            Author = "A",
            Available = 5,
            Category = new Category { Id = Guid.NewGuid() },
            ImageUrl = "old.jpg",
            SubImagesUrl = new List<string> { "s1.png" }
        };
        var newImageStream = new MemoryStream();
        var newImageFile = new FormFile(newImageStream, 0, 0, "n", "new.jpg");
        var req = new BookUpdateRequest
        {
            Id = book.Id,
            AddedQuantity = 0,
            CategoryId = book.Category.Id,
            NewImage = newImageFile
        };
        _bookRepo.Setup(r => r.GetByIdAsync(book.Id, "Category")).ReturnsAsync(book);
        _imageStorage.Setup(m => m.DeleteImageAsync("old.jpg", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _imageStorage.Setup(m => m.UploadImageAsync(newImageFile, It.IsAny<CancellationToken>())).ReturnsAsync("new.jpg");
        _bookRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()))
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _imageStorage.Verify(m => m.DeleteImageAsync("old.jpg", It.IsAny<CancellationToken>()), Times.Once);
        _imageStorage.Verify(m => m.UploadImageAsync(newImageFile, It.IsAny<CancellationToken>()), Times.Once);
        _bookRepo.Verify(r => r.Update(It.Is<Book>(b => b.ImageUrl == "new.jpg")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubImagesUrlRemoved_DeletesOnlyRemovedImages()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "B",
            Available = 5,
            Category = new Category { Id = Guid.NewGuid() },
            SubImagesUrl = new List<string> { "a.png", "b.png", "c.png" }
        };
        var req = new BookUpdateRequest
        {
            Id = book.Id,
            AddedQuantity = 0,
            CategoryId = book.Category.Id,
            SubImagesUrl = new List<string> { "a.png", "c.png" }
        };
        _bookRepo.Setup(r => r.GetByIdAsync(book.Id, "Category")).ReturnsAsync(book);
        _imageStorage.Setup(m => m.DeleteImageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _bookRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, (string?, string?)>>()))
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _imageStorage.Verify(m => m.DeleteImageAsync("b.png", It.IsAny<CancellationToken>()), Times.Once);
        _bookRepo.Verify(r => r.Update(It.Is<Book>(b => b.SubImagesUrl!.SequenceEqual(req.SubImagesUrl))), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNewSubImagesProvided_UploadsAndMergesCorrectly()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "B",
            Available = 5,
            Category = new Category { Id = Guid.NewGuid() },
            SubImagesUrl = new List<string> { "x1.png", "x2.png" }
        };
        var new1 = new FormFile(new MemoryStream(), 0, 0, "n1", "n1.png");
        var new2 = new FormFile(new MemoryStream(), 0, 0, "n2", "n2.png");
        var req = new BookUpdateRequest
        {
            Id = book.Id,
            AddedQuantity = 0,
            CategoryId = book.Category.Id,
            SubImagesUrl = new List<string> { "x1.png", "x2.png" },
            NewSubImages = new List<IFormFile> { new1, new2 },
            NewSubImagesPos = new List<int> { 1, 3 }
        };
        _bookRepo.Setup(r => r.GetByIdAsync(book.Id, "Category")).ReturnsAsync(book);
        _imageStorage.Setup(m => m.UploadImageAsync(new1, It.IsAny<CancellationToken>())).ReturnsAsync("u1.png");
        _imageStorage.Setup(m => m.UploadImageAsync(new2, It.IsAny<CancellationToken>())).ReturnsAsync("u2.png");
        _bookRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var expected = new List<string> { "x1.png", "u1.png", "x2.png", "u2.png" };
        _bookRepo.Verify(r => r.Update(It.Is<Book>(b => b.SubImagesUrl!.SequenceEqual(expected))), Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_BookNotFound_ReturnsFailure()
    {
        // Arrange
        var req = new BookUpdateRequest { Id = Guid.NewGuid() };
        _bookRepo
            .Setup(r => r.GetByIdAsync(req.Id, "Category"))
            .ReturnsAsync((Book)null);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain(BookErrors.BookCanNotFound);
    }

    [Fact]
    public async Task UpdateAsync_InvalidQuantity_ReturnsFailure()
    {
        // Arrange
        var book = new Book { Id = Guid.NewGuid(), Available = 1, Category = new Category { Id = Guid.NewGuid() } };
        var req = new BookUpdateRequest { Id = book.Id, AddedQuantity = -2 };
        _bookRepo
            .Setup(r => r.GetByIdAsync(book.Id, "Category"))
            .ReturnsAsync(book);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain(BookErrors.BookQuantityAddedInvalid);
    }

    [Fact]
    public async Task UpdateAsync_InvalidCategory_ReturnsFailure()
    {
        // Arrange
        var book = new Book { Id = Guid.NewGuid(), Available = 5, Category = new Category { Id = Guid.NewGuid() } };
        var req = new BookUpdateRequest { Id = book.Id, AddedQuantity = 0, CategoryId = Guid.NewGuid() };
        _bookRepo
            .Setup(r => r.GetByIdAsync(book.Id, "Category"))
            .ReturnsAsync(book);
        _categoryRepo
            .Setup(r => r.GetByIdAsync(req.CategoryId))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain(BookErrors.BookCanNotUpdateDueToInvalidCategory);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesAndLogsAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Old",
            Description = "Desc",
            Author = "Auth",
            Available = 5,
            Category = new Category { Id = categoryId }
        };
        var req = new BookUpdateRequest
        {
            Id = book.Id,
            Title = "New",
            Description = "NewDesc",
            Author = "NewAuth",
            AddedQuantity = 1,
            CategoryId = categoryId
        };
        _bookRepo
            .Setup(r => r.GetByIdAsync(book.Id, "Category"))
            .ReturnsAsync(book);
        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(new Category { Id = categoryId });
        _bookRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(req);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(BookCommandMessages.BookUpdatedSuccess);
        _bookRepo.Verify(r => r.Update(
            It.Is<Book>(b =>
                b.Title == "New" &&
                b.Description == "NewDesc" &&
                b.Author == "NewAuth" &&
                b.Available == 6 &&
                b.Category.Id == categoryId
            )
        ), Times.Once);
        _bookRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        _auditLogger.Verify(a => a.LogAsync(
            book.Id.ToString(),
            nameof(Book),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>>()
        ), Times.Once);
    }

}
