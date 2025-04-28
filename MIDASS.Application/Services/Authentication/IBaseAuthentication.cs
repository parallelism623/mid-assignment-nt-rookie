using MIDASS.Application.Commons.Models.Authentication;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.Services.Authentication;

public interface IBaseAuthentication
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    Task<Result<string>> LogoutAsync();
    Task<Result<string>> RegisterAsync(RegisterRequest registerRequest);

    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
