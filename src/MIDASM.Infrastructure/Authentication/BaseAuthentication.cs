using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.Commons.Options;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract.Enums;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.AuditLogMessage;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.Messages.ExceptionMessages;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using Rookies.Contract.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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
    private readonly IAuditLogger _auditLogger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseAuthentication(IJwtTokenServices jwtTokenServices,
        ICryptoServiceFactory cryptoServiceFactory,
        IOptions<JwtTokenOptions> jwtTokenOptions,
        IAuditLogger auditLogger,
        IUserRepository userRepository,
        IExecutionContext executionContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _jwtTokenOptions = jwtTokenOptions.Value;
        _jwtTokenServices = jwtTokenServices;
        _cryptoService = cryptoServiceFactory.SetCryptoAlgorithm(nameof(CryptoAlgorithmType.RSA));
        _executionContext = executionContext;
        _auditLogger = auditLogger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest)
    {
        User user = await ProcessLogIn(loginRequest);

        if(!user.IsVerifyCode)
        {
            return LoginResponse.CreateDefault();
        }

        string accessToken = _jwtTokenServices.GenerateAccessToken(user);
        string refreshToken = _jwtTokenServices.GenerateRefreshToken();

        await SaveUserRefreshToken(user, refreshToken);

        InitialExecutionContextValue(user);

        await HandleAuditLogUserLogin(user);

        return LoginResponse.Create(accessToken, refreshToken);
    }
    public async Task<Result<string>> ChangePasswordAsync(UserPasswordChangeRequest userPasswordChangeRequest)
    {
        var user = await _userRepository.GetByIdAsync(_executionContext.GetUserId());

        if (user == null)
        {
            return Result<string>.Failure(UserErrors.UserNotFound);
        }

        if (userPasswordChangeRequest.OldPassword != _cryptoService.Decrypt(user.Password))
        {
            return Result<string>.Failure(UserErrors.PasswordNotCorrect);
        }


        User.UpdatePassword(user, _cryptoService.Encrypt(userPasswordChangeRequest.Password));

        await _userRepository.SaveChangesAsync();

        return AuthenticationMessages.PasswordChangeSuccessFully;
    }
    public async Task<Result<string>> LogoutAsync()
    {
        var userId = _executionContext.GetUserId();

        ValidateUserIdNotEmpty(userId);

        var user = await _userRepository.GetByIdAsync(userId);

        ValidateUserNotNull(user);

        await RecallUserAccessAndRefreshTokenAsync(user!);

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
        user.RefreshTokenExpireTime = DateTime.UtcNow
            .AddDays(_jwtTokenOptions.ExpireRefreshTokenDays);

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

        var userId = GetUserIdFromTokenClaims(claimsPrincipal);

        var user = await _userRepository.GetByIdAsync(userId, nameof(User.Role));

        ValidateUserNotNull(user);

        ValidateRefreshToken(user!, refreshTokenRequest.RefreshToken);

        var accessToken = _jwtTokenServices.GenerateAccessToken(user);
        var refreshToken = _jwtTokenServices.GenerateRefreshToken();

        await SaveUserRefreshToken(user, refreshToken);

        return RefreshTokenResponse.Create(accessToken, refreshToken);
    }


    private async Task HandleAuditLogUserLogin(User user)
    {
        var changedProperties = new Dictionary<string, (string?, string?)>();
        changedProperties.Add(nameof(user.RefreshToken), (string.Empty, user.RefreshToken));
        changedProperties.Add(nameof(user.RefreshTokenExpireTime), (string.Empty, user.RefreshTokenExpireTime.ToString()));
        await _auditLogger.LogAsync(user.Id.ToString(),
            "Authentication",
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.UserLogin,
                user.Username,
                user.ModifiedAt.ToShortTime(),
                _httpContextAccessor.HttpContext?
                    .Request.Headers[nameof(HttpRequestHeader.UserAgent)].ToString() ?? string.Empty
                ), changedProperties);
    }

    private async Task HandleAuditLogUserLogout(User user)
    {
        var changedProperties = new Dictionary<string, (string?, string?)>();
        changedProperties.Add(nameof(user.RefreshToken), (user.RefreshToken, string.Empty));
        changedProperties.Add(nameof(user.RefreshTokenExpireTime), (user.RefreshTokenExpireTime.ToString(), string.Empty));
        await _auditLogger.LogAsync(user.Id.ToString(),
            "Authentication",
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.UserLogout,
                user.Username,
                user.ModifiedAt.ToShortTime(),
                _executionContext.GetUserAgent()
                ), changedProperties);
    }

    private void InitialExecutionContextValue(User user)
    {
        _executionContext.SetUser(new()
        {
            Username = user.Username,
            Id = user.Id
        });
    }

    private async Task RecallUserAccessAndRefreshTokenAsync(User user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.MinValue;
        _jwtTokenServices.RecallAccessToken();
        await _userRepository.SaveChangesAsync();
        await HandleAuditLogUserLogout(user);
    }

    private static void ValidateUserIdNotEmpty(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new UnAuthorizedException(ApplicationExceptionMessages.UserNull);
        }
    }

    private static void ValidateUserNotNull(User? user)
    {
        if (user == null)
        {
            throw new UnAuthorizedException(ApplicationExceptionMessages.UserNull);
        }
    }

    private static Guid GetUserIdFromTokenClaims(ClaimsPrincipal claimsPrincipal)
    {
        if (Guid.TryParse(claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sid)!.Value, out Guid userId))
        {
            throw new BadRequestException(ApplicationExceptionMessages.UserIdInExecutionContextInvalid);
        }
        return userId;
    }

    private void ValidateRefreshToken(User user, string refreshToken)
    {
        if (user.RefreshToken == null || DecryptData(user.RefreshToken) != refreshToken ||
            user.RefreshTokenExpireTime < DateTime.UtcNow)
        {
            throw new UnAuthorizedException(ApplicationExceptionMessages.InvalidRefreshToken);
        }
    }
}