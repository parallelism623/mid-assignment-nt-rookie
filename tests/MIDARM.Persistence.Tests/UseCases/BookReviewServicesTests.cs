using FluentAssertions;
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.UseCases;
using MockQueryable;
using Moq;

namespace MIDASM.Persistence.Services.Tests
{
    public class BookReviewServicesTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IBookBorrowingRequestRepository> _borrowReqRepoMock;
        private readonly Mock<IBookReviewRepository> _reviewRepoMock;
        private readonly Mock<IAuditLogger> _auditLoggerMock;
        private readonly Mock<IExecutionContext> _executionContextMock;
        private readonly BookReviewServices _service;

        public BookReviewServicesTests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _borrowReqRepoMock = new Mock<IBookBorrowingRequestRepository>();
            _reviewRepoMock = new Mock<IBookReviewRepository>();
            _auditLoggerMock = new Mock<IAuditLogger>();
            _executionContextMock = new Mock<IExecutionContext>();

            _service = new BookReviewServices(
                _reviewRepoMock.Object,
                _borrowReqRepoMock.Object,
                _userRepoMock.Object,
                _auditLoggerMock.Object,
                _bookRepoMock.Object,
                _executionContextMock.Object
            );
        }

        [Fact]
        public async Task CreateBookReviewAsync_BookNotExists_ReturnsFailure()
        {
            // Arrange
            var req = new CreateBookReviewRequest
            {
                ReviewerId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                Title = "Test",
                Content = "Content",
                DateReview = DateOnly.FromDateTime(DateTime.UtcNow),
                Rating = 5
            };
            _bookRepoMock.Setup(b => b.GetByIdAsync(req.BookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _service.CreateBookReviewAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookReviewErrors.BookNotExists);
        }

        [Fact]
        public async Task CreateBookReviewAsync_ReviewerNotExists_ReturnsFailure()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid() };
            var req = new CreateBookReviewRequest
            {
                ReviewerId = Guid.NewGuid(),
                BookId = book.Id,
                Title = "T",
                Content = "C",
                DateReview = DateOnly.FromDateTime(DateTime.UtcNow),
                Rating = 4
            };
            _bookRepoMock.Setup(b => b.GetByIdAsync(book.Id)).ReturnsAsync(book);
            _userRepoMock.Setup(u => u.GetByIdAsync(req.ReviewerId)).ReturnsAsync((User)null);

            // Act
            var result = await _service.CreateBookReviewAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookReviewErrors.ReviewerNotExists);
        }

        [Fact]
        public async Task CreateBookReviewAsync_UserNotBorrowed_ReturnsFailure()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid() };
            var user = new User { Id = Guid.NewGuid() };
            var req = new CreateBookReviewRequest
            {
                ReviewerId = user.Id,
                BookId = book.Id,
                Title = "T",
                Content = "C",
                DateReview = DateOnly.FromDateTime(DateTime.UtcNow),
                Rating = 3
            };
            _bookRepoMock.Setup(b => b.GetByIdAsync(book.Id)).ReturnsAsync(book);
            _userRepoMock.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _borrowReqRepoMock.Setup(r => r.FindByBookBorrowedOfUserAsync(user.Id, book.Id)).ReturnsAsync((BookBorrowingRequest)null);

            // Act
            var result = await _service.CreateBookReviewAsync(req);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().Contain(BookReviewErrors.UserHasNotBorrowedBook);
        }

        [Fact]
        public async Task CreateBookReviewAsync_ValidRequest_AddsReview_Saves_LogsAudit_ReturnsSuccess()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var book = new Book { Id = bookId };
            var user = new User { Id = userId, FirstName = "F", LastName = "L" };
            var borrowReq = new BookBorrowingRequest { Id = Guid.NewGuid(), RequesterId = userId };
            var req = new CreateBookReviewRequest
            {
                ReviewerId = userId,
                BookId = bookId,
                Title = "Good",
                Content = "Nice",
                DateReview = DateOnly.FromDateTime(DateTime.UtcNow),
                Rating = 5
            };
            _bookRepoMock.Setup(b => b.GetByIdAsync(bookId)).ReturnsAsync(book);
            _userRepoMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(user);
            _borrowReqRepoMock.Setup(r => r.FindByBookBorrowedOfUserAsync(userId, bookId)).ReturnsAsync(borrowReq);
            _reviewRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _auditLoggerMock.Setup(a => a.LogAsync(
                It.IsAny<string>(),
                nameof(BookReview),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, (string?, string?)>>()
            )).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateBookReviewAsync(req);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(BookReviewCommandMessages.CreateSuccess);
            _reviewRepoMock.Verify(r => r.Add(It.Is<BookReview>(br =>
                br.ReviewerId == userId && br.BookId == bookId && br.Rating == req.Rating
            )), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _auditLoggerMock.Verify(a => a.LogAsync(
                It.Is<string>(s => !string.IsNullOrEmpty(s)),
                nameof(BookReview),
                It.Is<string>(msg => msg.Contains("book review")),
                It.IsAny<Dictionary<string, (string?, string?)>>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsPagedReviewResponses()
        {
            // Arrange
            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            var review = BookReview.Create(
                Guid.NewGuid(), Guid.NewGuid(), "T", "C", now, 4);
            var list = new List<BookReview> { review };
            var mockQ = list.AsQueryable().BuildMock();
            _reviewRepoMock.Setup(r => r.GetQueryable()).Returns(mockQ);

            var queryParams = new BookReviewQueryParameters { Skip = 0, Take = 10 };

            // Act
            var result = await _service.GetAsync(queryParams);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var page = result.Data;
            page.Should().NotBeNull();
            page.TotalCount.Should().Be(1);
            page.Items.Should().HaveCount(1);
            var resp = page.Items.First();
            resp.Id.Should().Be(review.Id);
            resp.Title.Should().Be(review.Title);
            resp.Content.Should().Be(review.Content);
            resp.Rating.Should().Be(review.Rating);
            resp.DateReview.Should().Be(review.DateReview);
        }
    }
}
