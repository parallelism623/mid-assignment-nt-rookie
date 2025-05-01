using Mapster;
using Microsoft.Extensions.Options;
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.Commons.Options;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using Rookies.Contract.Exceptions;
using LoginRequest = MIDASM.Application.Commons.Models.Authentication.LoginRequest;
using RegisterRequest = MIDASM.Application.Commons.Models.Authentication.RegisterRequest;

namespace MIDASM.Infrastructure.Authentication;

public abstract class BaseAuthentication : IBaseAuthentication
{
    private readonly IJwtTokenServices _jwtTokenServices;
    private readonly ICryptoService _cryptoService;
    private readonly JwtTokenOptions _jwtTokenOptions;
    private readonly IUserRepository _userRepository;
    private readonly IExecutionContext _executionContext;

    protected BaseAuthentication(IJwtTokenServices jwtTokenServices,
        ICryptoServiceFactory cryptoServiceFactory,
        IOptions<JwtTokenOptions> jwtTokenOptions,
        IUserRepository userRepository,
        IExecutionContext executionContext)
    {
        _userRepository = userRepository;
        _jwtTokenOptions = jwtTokenOptions.Value;
        _jwtTokenServices = jwtTokenServices;
        _cryptoService = cryptoServiceFactory.SetCryptoAlgorithm("RSA");
        _executionContext = executionContext;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest)
    {
        User user = await ProcessLogIn(loginRequest);
        if(!user.IsVerifyCode)
        {
            return new LoginResponse() { AccessToken = "", RefreshToken = "" };
        }
        string token = _jwtTokenServices.GenerateAccessToken(user);
        string refreshToken = _jwtTokenServices.GenerateRefreshToken();
        await SaveUserRefreshToken(user, refreshToken);
        return new LoginResponse{ AccessToken = token, RefreshToken = refreshToken };
    }

    public async Task<Result<string>> LogoutAsync()
    {
        var userId = _executionContext.GetUserId();
        if (userId == Guid.Empty)
        {
            throw new UnAuthorizedException("User does not login, can not logout");
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnAuthorizedException("User does not login, can not logout");
        }
        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.MinValue;
        _jwtTokenServices.RecallAccessToken();
        await _userRepository.SaveChangesAsync();
        return AuthenticationMessages.LogoutSuccessfully;
    }
    public async Task<Result<string>> RegisterAsync(RegisterRequest registerRequest)
    {
        await ProcessRegister(registerRequest);
        return AuthenticationMessages.RegisterSuccess;
    }

    public abstract Task<User> ProcessLogIn(LoginRequest loginRequest);

    public abstract Task<User> ProcessRegister(RegisterRequest userRegisterModel);


    private async Task SaveUserRefreshToken(User user, string refreshToken)
    {
        user.RefreshToken = EncryptData(refreshToken);
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(_jwtTokenOptions.ExpireRefreshTokenDays);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }


    public string EncryptData(string data)
    {
        return _cryptoService.Encrypt(data);
    }

    public string DecryptData(string data)
    {
        return _cryptoService.Decrypt(data);
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var claimsPrincipal = _jwtTokenServices.ValidateAndDecode(refreshTokenRequest.AccessToken);

        Guid.TryParse(claimsPrincipal.FindFirst("sid")!.Value, out Guid userId);
        var user = await _userRepository.GetByIdAsync(userId, "Role");
        if(user == null)
        {
            throw new UnAuthorizedException("Invalid session");
        }
        if(user.RefreshToken == null || DecryptData(user.RefreshToken) != refreshTokenRequest.RefreshToken ||
            user.RefreshTokenExpireTime < DateTime.UtcNow)
        {
            throw new UnAuthorizedException("Invalid refresh token");
        }

        var accessToken = _jwtTokenServices.GenerateAccessToken(user);
        var refreshToken = _jwtTokenServices.GenerateRefreshToken();

        await SaveUserRefreshToken(user, refreshToken);
        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}