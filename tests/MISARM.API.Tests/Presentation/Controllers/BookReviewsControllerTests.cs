
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Application.UseCases.Interfaces;
using MIDASM.Contract.SharedKernel;
using MIDASS.Presentation.Controllers;
using Moq;

namespace MISARM.API.Tests.Presentation.Controllers;

public class BookReviewsControllerTests
{
    private readonly Mock<IBookReviewServices> _bookReviewServices;
    private readonly BookReviewsController _bookReviewController;
    public BookReviewsControllerTests()
    {
        _bookReviewServices = new();
        _bookReviewController = new(_bookReviewServices.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCreateSuccess_ShouldReturnCorrespondingResult()
    {
        // Arrange 
        var fakeResult = Result<string>.Success("success");
        _bookReviewServices.Setup(b => b.CreateBookReviewAsync(It.IsAny<CreateBookReviewRequest>()))
            .ReturnsAsync(fakeResult);


        // Act

        var result = await _bookReviewController.CreateAsync(It.IsAny<CreateBookReviewRequest>());

        // Assert

        _bookReviewServices.Verify(b => b.CreateBookReviewAsync(It.IsAny<CreateBookReviewRequest>()), Times.Once());

        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(fakeResult);
    }
    [Fact]
    public async Task GetPaginationItemsAsync_WhenServiceReturnsSuccess_ReturnsOkWithData()
    {
        // Arrange


        var expectedPage = PaginationResult<BookReviewDetailResponse>.Create(2,
            new List<BookReviewDetailResponse>
            {
                new BookReviewDetailResponse { Id = Guid.NewGuid(), Title = "Review A" },
                new BookReviewDetailResponse { Id = Guid.NewGuid(), Title = "Review B" },
            });


        var fakeResult = Result<PaginationResult<BookReviewDetailResponse>>.Success(expectedPage);

        _bookReviewServices
            .Setup(s => s.GetAsync(It.IsAny<BookReviewQueryParameters>()))
            .ReturnsAsync(fakeResult);

        // Act
        var actionResult = await _bookReviewController.GetPaginationItemsAsync(It.IsAny<BookReviewQueryParameters>());

        // Assert
        var okResult = actionResult
            .Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Subject;

        var returnedPage = okResult.Value
            .Should().BeAssignableTo<Result<PaginationResult<BookReviewDetailResponse>>>()
            .Subject.Data;
        returnedPage.Should().NotBeNull();
        returnedPage.TotalCount
            .Should().Be(expectedPage.TotalCount);

        returnedPage.Items
            .Should().HaveCount(expectedPage.Items?.Count() ?? 0);

        returnedPage.Items.Select(i => i.Title)
            .Should().Contain(new List<string>{ "Review A", "Review B" });


        _bookReviewServices.Verify(s => s.GetAsync(
            It.IsAny<BookReviewQueryParameters>()), Times.Once);

    }
}
