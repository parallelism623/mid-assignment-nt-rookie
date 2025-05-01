using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.Services.Authentication;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/auth")]
public class AuthenticationController : ApiBaseController
{

    private readonly IApplicationAuthentication _applicationAuthentication;

    public AuthenticationController(IApplicationAuthentication applicationAuthentication)
    {
        _applicationAuthentication = applicationAuthentication;
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        var result = await _applicationAuthentication.LoginAsync(loginRequest);

        return ProcessResult(result);
    }

    [HttpPost]
    [Route("logout")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> LogoutAsync()
    {
        var result = await _applicationAuthentication.LogoutAsync();

        return ProcessResult(result);
    }
        
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        var result = await _applicationAuthentication.RegisterAsync(registerRequest);

        return ProcessResult(result);
    }
    [HttpPost]
    [Route("token-refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest registerRequest)
    {
        var result = await _applicationAuthentication.RefreshTokenAsync(registerRequest);

        return ProcessResult(result);
    }

    [HttpPost]
    [Route("email-confirm")]
    public async Task<IActionResult> CreateVerifyCode([FromBody] EmailConfirmRequest creatVerifyCodeRequset)
    {
        var result = await _applicationAuthentication.ConfirmEmailAsync(creatVerifyCodeRequset);

        return ProcessResult(result);
    }
    [HttpPost]
    [Route("email-confirm-refresh")]
    public async Task<IActionResult> RefreshEmailConfirm([FromBody] RefreshEmailConfirmTokenRequest refreshEmailConfirm)
    {
        var result = await _applicationAuthentication.RefreshEmailConfirmAsync(refreshEmailConfirm);

        return ProcessResult(result);
    }


}
