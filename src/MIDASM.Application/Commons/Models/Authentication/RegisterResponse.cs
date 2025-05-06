
namespace MIDASM.Application.Commons.Models.Authentication;

public class RegisterResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
