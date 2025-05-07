using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.Commons.Options;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.Services.Mail;
using MIDASM.Contract.Constants;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.Messages.ExceptionMessages;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using Org.BouncyCastle.Operators;
using Rookies.Contract.Exceptions;
using System.Security.Cryptography;
using LoginRequest = MIDASM.Application.Commons.Models.Authentication.LoginRequest;
using RegisterRequest = MIDASM.Application.Commons.Models.Authentication.RegisterRequest;
namespace MIDASM.Infrastructure.Authentication;

public class ApplicationAuthentication : BaseAuthentication, IApplicationAuthentication
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> _backgroundTaskQueue;
    public ApplicationAuthentication(IJwtTokenServices jwtTokenServices, 
        ICryptoServiceFactory cryptoServiceFactory,
        IOptions<JwtTokenOptions> jwtTokenOptions,
        IUserRepository userRepository,
        IExecutionContext executionContext, 
        IRoleRepository roleRepository, 
        IMemoryCache memoryCache, 
        IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> backgroundTaskQueue,
        IAuditLogger auditLogger,
        IHttpContextAccessor httpContextAccessor) 
        : base(jwtTokenServices, cryptoServiceFactory, jwtTokenOptions, auditLogger,
        userRepository, executionContext, httpContextAccessor)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _memoryCache = memoryCache;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<string>> ConfirmEmailAsync(EmailConfirmRequest emailConfirmRequest)
    {
        var user = await _userRepository.GetByUsernameAsync(emailConfirmRequest.Username);
        if (user == null || user.Email == null)
        {
            throw new BadRequestException(ApplicationExceptionMessages.UserNameInvalid);
        }

        var codeInMemory = GetVerifyCodeUserInMemory(user);

        ValidateVerifyCode(codeInMemory, emailConfirmRequest.Code);

        await UpdateUserVerifiedStatusAsync(user);

        return AuthenticationMessages.SendVerifyCodeSuccess;
    }

    public override async Task<User> ProcessLogIn(LoginRequest loginRequest)
    {
        var user = await GetUserOfLoginRequestAsync(loginRequest);

        ValidateUserNotNull(user);

        ComparePasswordWithRequestPassword(user.Password, loginRequest.Password);

        await CheckUserVerifyAsync(user);

        return user;
    }

    public override async Task<User> ProcessRegister(RegisterRequest userRegisterModel)
    {
        await ValidateUserExists(userRegisterModel.Username, userRegisterModel.Email);

        var userRole = await _roleRepository.GetByNameAsync(nameof(RoleName.User));

        ValidateRoleUserNotNull(userRole);

        var newUser = User.Create(userRegisterModel.Email, userRegisterModel.Username, EncryptData(userRegisterModel.Password),
            userRegisterModel.FirstName, userRegisterModel.LastName, userRegisterModel.PhoneNumber, userRole!.Id);

        await AddUserIntoStorageAsync(newUser);

        await SendEmailConfirmCode(newUser!);
        
        return newUser;
    }

    private async Task SendEmailConfirmCode(User user)
    {
        var code = GenerateVerificationCode();

        var body = await GetMailVerifyCodeTemplate(user, code);

        var subject = GetTitleOfVerifyCodeMail(user.Username);

        await HandleSendVerifyCodeMail(user.Email, subject, body);

        SetVerifyCodeIntoMemoryCache(user.Id, code);
    }
    private Task<User?> GetUserOfLoginRequestAsync(LoginRequest loginRequest)
    {
        VerifyUsernameNotNull(loginRequest.Username);

        return _userRepository.GetByUsernameAsync(loginRequest.Username!, nameof(User.Role));
    }

    private static string GenerateVerificationCode()
    {
        int code = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return code.ToString("D6");
    }

    private static async Task<string> GetMailVerifyCodeTemplate(User user, string code)
    {
        string content = await FileHelper.GetMailTemplateFile(MailTemplateHelper.MailTemplateVerifyNewAccount);

        return content.Replace("<<<FullName>>>", user.FirstName + " " + user.LastName)
                      .Replace("<<<{{Code}}>>>", code)
                      .Replace("<<<ExpiryMinutes>>>", "5");
    }

    private async Task HandleSendVerifyCodeMail(string toEmail, string subject, string body)
    {
        await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (servicesProvider, cts) =>
        {
            var mailServices = servicesProvider.GetRequiredService<IMailServices>();

            await mailServices.SendMailAsync(toEmail, subject, body, cancellationToken: cts);
        });
    }

    public async Task<Result<string>> RefreshEmailConfirmAsync(RefreshEmailConfirmTokenRequest refreshEmailConfirm)
    {
        var user = await _userRepository.GetByUsernameAsync(refreshEmailConfirm.Username);
        if(user == null || user.Email == null)
        {
            throw new BadRequestException("User invalid");
        }
        await SendEmailConfirmCode(user);
        return AuthenticationMessages.RefreshEmailConfirmSuccess;
    }


    private async Task UpdateUserVerifiedStatusAsync(User user)
    {
        user.IsVerifyCode = true;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    private string GetVerifyCodeUserInMemory(User user)
    {
        return _memoryCache.Get(string.Format(CacheKey.RegisterVerifyCode, user.Id))?
            .ToString() ?? string.Empty;
    }

    private static void ValidateVerifyCode(string verifyCodeInMemory, string requestCodeVerify)
    {
        if (string.IsNullOrEmpty(verifyCodeInMemory) || verifyCodeInMemory != requestCodeVerify)
        {
            throw new BadRequestException(ApplicationExceptionMessages.EmailConfirmCodeInvalid);
        }
    }

    private void ComparePasswordWithRequestPassword(string password, string requestPassword)
    {
        if (DecryptData(password) != requestPassword)
        {
            throw new BadRequestException(ApplicationExceptionMessages.PasswordIncorrect);
        }
    }

    private async Task CheckUserVerifyAsync(User user)
    {
        if (!user.IsVerifyCode)
        {
            await SendEmailConfirmCode(user);
        }
    }

    private static void ValidateUserNotNull(User? user)
    {
        if (user == null)
        {
            throw new BadRequestException(ApplicationExceptionMessages.EmailOrUsernameIncorrect);
        }
    }

    private async Task ValidateUserExists(string userName, string email)
    {
        var userByUsername = await _userRepository.GetByUsernameAsync(userName, nameof(User.Role));
        if (userByUsername != null)
        {
            throw new BadRequestException(ApplicationExceptionMessages.UsernameAlreadyExists);
        }

        var userByEmail = await _userRepository.GetByEmailAsync(email);

        if (userByEmail != null)
        {
            throw new BadRequestException(ApplicationExceptionMessages.EmailAlreadyExists);
        }
    }

    private static void ValidateRoleUserNotNull(Role? userRole)
    {
        if (userRole == null)
        {
            throw new BadRequestException(ApplicationExceptionMessages.RoleUserDoesNotExists);
        }
    }

    private async Task AddUserIntoStorageAsync(User user)
    {
        _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();
    }

    private void SetVerifyCodeIntoMemoryCache(Guid userId, string code)
    {
        _memoryCache.Set(string.Format(CacheKey.RegisterVerifyCode, userId), code, absoluteExpiration: DateTimeOffset.Now.AddMinutes(5));
    }

    private static string GetTitleOfVerifyCodeMail(string userName)
    {
        return $"Verify account #{userName}";
    }

    private static void VerifyUsernameNotNull(string? userName)
    {

        if (string.IsNullOrEmpty(userName))
        {
            throw new BadRequestException(ApplicationExceptionMessages.UserOrEmailMustBeProvided);
        }
        
    }
}