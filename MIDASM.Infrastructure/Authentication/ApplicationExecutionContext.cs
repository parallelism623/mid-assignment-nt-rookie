
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.Authentication;

namespace MIDASM.Infrastructure.Authentication;

public class ApplicationExecutionContext : IExecutionContext
{
    private UserExecutionContext? _userExecutionContext;
    private string? _accessToken;
    private Guid? _jti;
    public void SetUser(UserExecutionContext user)
    {
        _userExecutionContext = user;
    }



    public string GetRole() => _userExecutionContext?.Role?.Name ?? string.Empty;

    public string GetUserName() => _userExecutionContext?.Username ?? string.Empty;
    public Guid GetUserId() => _userExecutionContext?.Id ?? Guid.Empty;
    public Guid GetJti()
    {
        return _jti ?? Guid.Empty;
    }

    public void SetJti(Guid jti)
    {
        _jti = jti;
    }

    public string GetAccessToken()
    {
        return _accessToken ?? string.Empty;
    }

    public void SetAccessToken(string token)
    {
        _accessToken = token;
    }

    public string GetUserMail()
    {
        return _userExecutionContext?.Email ?? string.Empty;
    }
}
