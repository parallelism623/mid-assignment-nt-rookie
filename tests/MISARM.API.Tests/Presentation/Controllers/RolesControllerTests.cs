using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using MIDASM.Application.Commons.Models.Authentication;

namespace MIDASM.API.Tests.Controllers
{
    public class RolesControllerTests
    {
        private readonly Mock<IRoleServices> _roleServicesMock;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            _roleServicesMock = new Mock<IRoleServices>();
            _controller = new RolesController(_roleServicesMock.Object);
        }

        [Fact]
        public async Task GetAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var roles = new List<RoleResponse> ();
            var fakeResult = Result<List<RoleResponse>>.Success(roles);
            _roleServicesMock
                .Setup(s => s.GetAsync())
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.GetAsync();

            // Assert
            _roleServicesMock.Verify(s => s.GetAsync(), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value
                .Should()
                .Be(fakeResult);
        }
    }
}
