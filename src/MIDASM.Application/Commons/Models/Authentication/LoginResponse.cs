
namespace MIDASM.Application.Commons.Models.Authentication;

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;

    public static LoginResponse CreateDefault()
    {
        return new() { AccessToken = string.Empty, RefreshToken = string.Empty };
    }

    public static LoginResponse Create(string accessToken, string refreshToken)
    {
        return new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
