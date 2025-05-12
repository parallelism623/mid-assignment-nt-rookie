
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MIDASM.Contract.SharedKernel;
using MIDASS.Presentation.Controllers;
using MISARM.API.Tests.DataTestSources;
using Moq;
using Rookies.Contract.Exceptions;

namespace MISARM.API.Tests.Presentation.Controllers
{
    public class ApiBaseControllerTests
    {
        private class ApiBaseControllerStub : ApiBaseController
        {
            public IActionResult ProcessResultStub<T>(Result<T> result) => ProcessResult(result);

            public IActionResult ProcessFileResultStub(byte[] dataBytes, string contentType, string fileName = default!) 
                => ProcessFileResult(dataBytes, contentType, fileName);
        }
        [Theory]
        [ClassData(typeof(ApiBaseControllerTestDatas))]
        public void ProcessResult_WhenReceivedResult_ShouldReturnSameStatusCodeResult(Result<string> result)
        {
            // Arrange

            var apiBaseController = GetApiBaseController;

            // Action

            var actualResult = apiBaseController.ProcessResultStub(result);

            // Assert

            actualResult.Should().NotBeNull();
            var statusResult = actualResult as IStatusCodeActionResult;
            statusResult.Should().NotBeNull();
            statusResult.StatusCode.Should().Be(result.StatusCode);
        }

        [Fact]
        public void ProcessResult_WhenReceivedNullResult_ShouldThrowBadRequestException()
        {
            // Arrange

            var apiBaseController = GetApiBaseController;

            // Action

            var exceptionResult = () => apiBaseController.ProcessResultStub<string>(default!);

            // Assert


            exceptionResult.Should()
               .Throw<BadRequestException>()
               .Which
               .Message
               .Should().Be("Response api must be not null");
        }

        [Theory]
        [ClassData(typeof(ApiBaseControllerTestFileDatas))]
        public void ProcessResult_WhenReceivedFileResult_ShouldReturnFileResult(byte[] content, string contentType, string fileName = default!)
        {
            // Arrange

            var apiBaseController = GetApiBaseController;

            // Action

            var result = apiBaseController.ProcessFileResultStub(content, contentType, fileName);

            // Assert


            var fileResult = result
                .Should()
                .BeOfType<FileContentResult>()   
                .Which;                           

            fileResult.ContentType
                .Should().Be(contentType);

            fileResult.FileDownloadName
                .Should().Be(fileName);

            fileResult.FileContents
                .Should().Equal(content);
        }

        [Fact]
        public void ProcessResult_WhenReceivedFileContentNull_ShouldThrowBadRequestException()
        {
            // Arrange

            var apiBaseController = GetApiBaseController;

            // Action

            var exceptionResult = () => apiBaseController.ProcessFileResultStub(null!, "test");

            // Assert

            exceptionResult.Should()
                .Throw<BadRequestException>()
                .Which
                .Message.Should().Be("Response api must be not null");
 
        }

        [Fact]
        public void ProcessResult_WhenReceivedFileTypeInvalid_ShouldThrowBadRequestException()
        {
            // Arrange

            var apiBaseController = GetApiBaseController;

            // Action

            var exceptionResult = () => apiBaseController.ProcessFileResultStub([], "");

            // Assert

            exceptionResult.Should()
                .Throw<BadRequestException>()
                .Which
                .Message.Should().Be("Response file type invalid");

        }
        private ApiBaseControllerStub GetApiBaseController => new ApiBaseControllerStub();
    }
}
