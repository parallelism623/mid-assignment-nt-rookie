using Microsoft.Extensions.Options;
using MIDASS.Application.Commons.Options;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.Services.Crypto;
using MIDASS.Application.Services.Mail;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;
using Rookies.Contract.Exceptions;
using LoginRequest = MIDASS.Application.Commons.Models.Authentication.LoginRequest;
using RegisterRequest = MIDASS.Application.Commons.Models.Authentication.RegisterRequest;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using MIDASS.Contract.Messages.Commands;
using Microsoft.Extensions.Caching.Memory;
using MIDASS.Contract.Constants;
using MIDASS.Application.Commons.Models.Authentication;
using MIDASS.Application.Services.HostedServices.Abstract;
using MIDASS.Infrastructure.HostedServices.Abstract;
using Microsoft.Extensions.DependencyInjection;
namespace MIDASS.Infrastructure.Authentication;

public class ApplicationAuthentication : BaseAuthentication, IApplicationAuthentication
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IWebHostEnvironment _env;
    private readonly IMemoryCache _memoryCache;
    private readonly IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> _backgroundTaskQueue;
    public ApplicationAuthentication(IJwtTokenServices jwtTokenServices, ICryptoServiceFactory cryptoServiceFactory,
        IOptions<JwtTokenOptions> jwtTokenOptions, IWebHostEnvironment env, IUserRepository userRepository,
        IExecutionContext executionContext, IRoleRepository roleRepository, IMemoryCache memoryCache, IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> backgroundTaskQueue) : base(jwtTokenServices, cryptoServiceFactory, jwtTokenOptions,
        userRepository, executionContext)
    {
        _env = env;
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
            throw new BadRequestException("Username invalid");
        }
        var codeInMem = _memoryCache.Get(string.Format(CacheKey.RegisterVerifyCode, user.Id))?.ToString() ?? string.Empty;
        if(string.IsNullOrEmpty(codeInMem) || codeInMem != emailConfirmRequest.Code)
        {
            throw new BadRequestException("Email confirm code invalid");
        }
        user.IsVerifyCode = true;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return AuthenticationMessages.SendVerifyCodeSuccess;
    }

    public override async Task<User> ProcessLogIn(LoginRequest loginRequest)
    {
        var user = await GetUserOfLoginRequestAsync(loginRequest);
        if (user == null)
        {
            throw new BadRequestException("Username or email incorrect");
        }

        var password = DecryptData(user.Password);
        if (password != loginRequest.Password)
        {
            throw new BadRequestException("Password incorrect");
        }
        if(!user.IsVerifyCode)
        {
            await SendEmailConfirmCode(user);
        }
        return user;
    }


    public override async Task<User> ProcessRegister(RegisterRequest userRegisterModel)
    {
        var user = await _userRepository.GetByUsernameAsync(userRegisterModel.Username, "Role");
        if (user != null)
        {
            throw new BadRequestException("Username already exists");
        }

        var userByEmail = await _userRepository.GetByEmailAsync(userRegisterModel.Email);

        if (userByEmail != null)
        {
            throw new BadRequestException("Email already exists");
        }
        var userRole = await _roleRepository.GetByNameAsync(RoleName.User.ToString());
        if (userRole == null)
        {
            throw new BadRequestException("Role user does not exists");
        }

        var newUser = User.Create(userRegisterModel.Email, userRegisterModel.Username, EncryptData(userRegisterModel.Password),
            userRegisterModel.FirstName, userRegisterModel.LastName, userRegisterModel.PhoneNumber, userRole.Id);

        _userRepository.Add(newUser);
        await _userRepository.SaveChangesAsync();
 
        await SendEmailConfirmCode(newUser!);
        
        return newUser;
    }

    private async Task SendEmailConfirmCode(User user)
    {
        var code = GenerateVerificationCode();
        var body = await GetMailVerifyCodeTemplate(user, code);
        var subject = $"Verify account #{user.Username}";
        await HandleSendVerifyCodeMail(user.Email, subject, body);
        _memoryCache.Set(string.Format(CacheKey.RegisterVerifyCode, user.Id), code, absoluteExpiration: DateTimeOffset.Now.AddMinutes(5));
    }
    private Task<User?> GetUserOfLoginRequestAsync(LoginRequest loginRequest)
    {
        if (!string.IsNullOrEmpty(loginRequest.Username))
        {
            return _userRepository.GetByUsernameAsync(loginRequest.Username, "Role");
        }

        throw new BadRequestException("Username or email should be provided");
    }

    private static string GenerateVerificationCode()
    {
        int code = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return code.ToString("D6");
    }

    private async Task<string> GetMailVerifyCodeTemplate(User user, string code)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "VerifyCode.html");
        string content = await System.IO.File.ReadAllTextAsync(templatePath) ?? "";

        return content.Replace("<<<FullName>>>", user.FirstName + " " + user.LastName)
                      .Replace("<<<Code>>>", code)
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
}