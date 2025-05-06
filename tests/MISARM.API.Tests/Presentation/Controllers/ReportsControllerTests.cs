
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.UseCases;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Contract.SharedKernel;
using MISARM.API.Tests.TestHelpers;

namespace MIDASM.API.Tests.Controllers
{
    public class ReportsControllerTests
    {
        private readonly Mock<IReportServices> _reportServicesMock;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            _reportServicesMock = new Mock<IReportServices>();
            _controller = new ReportsController(_reportServicesMock.Object);
        }

        [Fact]
        public async Task GetBookBorrowingReport_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new BookBorrowingReportQueryParameters();
            var data = new List<BookBorrowingReportResponse>();
            var paginationData = PaginationResult<BookBorrowingReportResponse>.Create(0, data);
            var fakeResult = Result<PaginationResult<BookBorrowingReportResponse>>.Success(paginationData);
            _reportServicesMock
                .Setup(s => s.GetBookBorrowingReportAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetBookBorrowingReport(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetBookBorrowingReportAsync(query), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value
                .Should()
                .Be(fakeResult);
        }

        [Fact]
        public async Task GetCategoryReport_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new CategoryReportQueryParameters();
            var data = new List<CategoryReportResponse>();
            var paginationData = PaginationResult<CategoryReportResponse>.Create(0, data);
            var fakeResult = Result< PaginationResult<CategoryReportResponse>>.Success(paginationData);
            _reportServicesMock
                .Setup(s => s.GetCategoryReportAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetCategoryReport(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetCategoryReportAsync(query), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value
                .Should()
                .Be(fakeResult);
        }

        [Fact]
        public async Task GetUserReport_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var query = new UserEngagementReportQueryParameters();
            var fakeResult = ResultPaginationCreateHelper.CreateStubResult<UserReportResponse>();
            _reportServicesMock
                .Setup(s => s.GetUserReportAsync(query))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetUserReport(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetUserReportAsync(query), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value
                .Should()
                .Be(fakeResult);
        }
    }
}
