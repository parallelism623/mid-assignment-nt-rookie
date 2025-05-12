using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Services.Authentication;

public interface IBaseAuthentication
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    Task<Result<string>> LogoutAsync();
    Task<Result<string>> RegisterAsync(RegisterRequest registerRequest);
    Task<Result<string>> ChangePasswordAsync(UserPasswordChangeRequest userPasswordChangeRequest);
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
