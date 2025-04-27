
namespace MIDASS.Application.Commons.Models.Authentication;

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
