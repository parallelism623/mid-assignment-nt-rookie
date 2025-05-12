using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Contract.SharedKernel;
using System.Reflection.Metadata;
using MISARM.API.Tests.TestHelpers;
using MIDASM.Application.UseCases.Interfaces;
using MIDASS.Presentation.Controllers;

namespace MIDASM.API.Tests.Controllers
{
    public class ExportsControllerTests
    {
        private readonly Mock<IExportFactory> _exportFactoryMock;
        private readonly Mock<IReportServices> _reportServicesMock;
        private readonly ExportsController _controller;

        public ExportsControllerTests()
        {
            _exportFactoryMock = new Mock<IExportFactory>();
            _reportServicesMock = new Mock<IReportServices>();
            _controller = new ExportsController(_exportFactoryMock.Object, _reportServicesMock.Object);
        }

        [Fact]
        public async Task GetExportFileReportUserEngagementAsync_ReturnsFileContentResult()
        {
            // Arrange
            var exportType = "ExportToExcel";
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var toDate = fromDate.AddDays(1);
            var top = 5;
            var query = new ExportReportUserEngagementQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate,
                Top = top,
                ExportType = exportType
            };
            var reportResult = ResultPaginationCreateHelper.CreateStubResult<UserReportResponse>();
            _reportServicesMock
                .Setup(s => s.GetUserReportAsync(It.Is<UserEngagementReportQueryParameters>(p =>
                    p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)))
                .ReturnsAsync(reportResult);

            var expectedBytes = new byte[] { 0x10, 0x20 };
            var expectedContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var expectedFileName = "users.xlsx";
            var exportResponse = new ExportResponse
            {
                DataBytes = expectedBytes,
                ContentType = expectedContentType,
                FileName = expectedFileName
            };
            _exportFactoryMock
                .Setup(f => f.ExportFile(exportType, It.IsAny<ExportRequest<UserReportResponse>>()))
                .Returns(exportResponse);

            // Act
            var result = await _controller.GetExportFileReportUserEngagementAsync(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetUserReportAsync(
                It.Is<UserEngagementReportQueryParameters>(p => p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)), Times.Once);
            _exportFactoryMock.Verify(f => f.ExportFile(exportType,
                It.IsAny<ExportRequest<UserReportResponse>>()), Times.Once);

            var fileResult = result.Should().BeOfType<FileContentResult>().Which;
            fileResult.FileContents.Should().Equal(expectedBytes);
            fileResult.ContentType.Should().Be(expectedContentType);
            fileResult.FileDownloadName.Should().Be(expectedFileName);
        }

        [Fact]
        public async Task GetExportFileReportCategoriesAsync_ReturnsFileContentResult()
        {
            // Arrange
            var exportType = "ExportToPdf";
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var top = 3;
            var query = new ExportReportCategoriesQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate,
                Top = top,
                ExportType = exportType
            };
            var reportResult = ResultPaginationCreateHelper.CreateStubResult<CategoryReportResponse>();
            _reportServicesMock
                .Setup(s => s.GetCategoryReportAsync(It.Is<CategoryReportQueryParameters>(p =>
                    p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)))
                .ReturnsAsync(reportResult);

            var expectedBytes = new byte[] { 0xAA };
            var expectedContentType = "application/pdf";
            var expectedFileName = "categories.pdf";
            var exportResponse = new ExportResponse
            {
                DataBytes = expectedBytes,
                ContentType = expectedContentType,
                FileName = expectedFileName
            };
            _exportFactoryMock
                .Setup(f => f.ExportFile(exportType, It.IsAny<ExportRequest<CategoryReportResponse>>()))
                .Returns(exportResponse);

            // Act
            var result = await _controller.GetExportFileReportCategoriesAsync(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetCategoryReportAsync(
                It.Is<CategoryReportQueryParameters>(p => p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)), Times.Once);
            _exportFactoryMock.Verify(f => f.ExportFile(exportType,
                It.Is<ExportRequest<CategoryReportResponse>>(req => req.DataExport == reportResult.Data!.Items)), Times.Once);

            var fileResult = result.Should().BeOfType<FileContentResult>().Which;
            fileResult.FileContents.Should().Equal(expectedBytes);
            fileResult.ContentType.Should().Be(expectedContentType);
            fileResult.FileDownloadName.Should().Be(expectedFileName);
        }

        [Fact]
        public async Task GetExportFileReportBookBorrowingAsync_ReturnsFileContentResult()
        {
            // Arrange
            var exportType = "ExportToExcel";
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(-1));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var top = 10;
            var query = new ExportReportBookBorrowingQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate,
                Top = top,
                ExportType = exportType
            };
            var reportResult = ResultPaginationCreateHelper.CreateStubResult<BookBorrowingReportResponse>();
            _reportServicesMock
                .Setup(s => s.GetBookBorrowingReportAsync(It.Is<BookBorrowingReportQueryParameters>(p =>
                    p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)))
                .ReturnsAsync(reportResult);

            var expectedBytes = new byte[] { 0x0F, 0xF0 };
            var expectedContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var expectedFileName = "borrowings.xlsx";
            var exportResponse = new ExportResponse
            {
                DataBytes = expectedBytes,
                ContentType = expectedContentType,
                FileName = expectedFileName
            };
            _exportFactoryMock
                .Setup(f => f.ExportFile(exportType, It.IsAny<ExportRequest<BookBorrowingReportResponse>>()))
                .Returns(exportResponse);

            // Act
            var result = await _controller.GetExportFileReportBookBorrowingAsync(query);

            // Assert
            _reportServicesMock.Verify(s => s.GetBookBorrowingReportAsync(
                It.Is<BookBorrowingReportQueryParameters>(p => p.FromDate == fromDate && p.ToDate == toDate && p.Top == top)), Times.Once);
            _exportFactoryMock.Verify(f => f.ExportFile(exportType,
                It.Is<ExportRequest<BookBorrowingReportResponse>>(req => req.DataExport == reportResult.Data!.Items)), Times.Once);

            var fileResult = result.Should().BeOfType<FileContentResult>().Which;
            fileResult.FileContents.Should().Equal(expectedBytes);
            fileResult.ContentType.Should().Be(expectedContentType);
            fileResult.FileDownloadName.Should().Be(expectedFileName);
        }
    }
}
