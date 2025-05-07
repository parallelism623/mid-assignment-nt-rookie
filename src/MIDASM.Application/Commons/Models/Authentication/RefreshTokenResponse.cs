
namespace MIDASM.Application.Commons.Models.Authentication;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;

    public static RefreshTokenResponse Create(string accessToken, string refreshToken)
    {
        return new() { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
