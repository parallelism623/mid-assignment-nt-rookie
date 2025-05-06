using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using MIDASM.API.Presentation.Controllers;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Contract.SharedKernel;
using Azure.Core;

namespace MIDASM.API.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IApplicationAuthentication> _authMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _authMock = new Mock<IApplicationAuthentication>();
            _controller = new AuthenticationController(_authMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new LoginRequest { Username = "user", Password = "pass" };
            Result<LoginResponse> fakeResult = new LoginResponse() { AccessToken = "access_token", RefreshToken = "refresh_token" };
            _authMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.LoginAsync(request);

            // Assert
            _authMock.Verify(s => s.LoginAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task LogoutAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            Result<string> fakeResult = "logout-success";
            _authMock
                .Setup(s => s.LogoutAsync())
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.LogoutAsync();

            // Assert
            _authMock.Verify(s => s.LogoutAsync(), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task RegisterAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new RegisterRequest { Email = "a@b.com", Password = "pass123" };
            Result<string> fakeResult = "register-success";
            _authMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.RegisterAsync(request);

            // Assert
            _authMock.Verify(s => s.RegisterAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new RefreshTokenRequest { AccessToken = "old-token", RefreshToken = "old-refresh-token"};
            Result<RefreshTokenResponse> fakeResult = new RefreshTokenResponse {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            };
            _authMock
                .Setup(s => s.RefreshTokenAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.RefreshTokenAsync(request);

            // Assert
            _authMock.Verify(s => s.RefreshTokenAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task CreateVerifyCode_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new EmailConfirmRequest { Username = "a@b.com", Code = "123456"};
            Result<string> fakeResult = "verify-code";
            _authMock
                .Setup(s => s.ConfirmEmailAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.CreateVerifyCode(request);

            // Assert
            _authMock.Verify(s => s.ConfirmEmailAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task RefreshEmailConfirm_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new RefreshEmailConfirmTokenRequest { Username = "user-name" };
            Result<string> fakeResult = "new-email-token";
            _authMock
                .Setup(s => s.RefreshEmailConfirmAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.RefreshEmailConfirm(request);

            // Assert
            _authMock.Verify(s => s.RefreshEmailConfirmAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenCalled_InvokesServiceAndReturnsOk()
        {
            // Arrange
            var request = new UserPasswordChangeRequest {OldPassword = "old", Password = "new" };
            Result<string> fakeResult = "password-changed";
            _authMock
                .Setup(s => s.ChangePasswordAsync(request))
                .ReturnsAsync(fakeResult);

            // Act
            var actionResult = await _controller.ChangePasswordAsync(request);

            // Assert
            _authMock.Verify(s => s.ChangePasswordAsync(request), Times.Once);
            actionResult
                .Should()
                .BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(fakeResult);
        }
    }
}
