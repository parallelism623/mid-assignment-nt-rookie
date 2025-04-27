using Microsoft.Extensions.Options;
using MIDASS.Application.Commons.Options;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.Services.Crypto;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;
using Rookies.Contract.Exceptions;
using LoginRequest = MIDASS.Application.Commons.Models.Authentication.LoginRequest;
using RegisterRequest = MIDASS.Application.Commons.Models.Authentication.RegisterRequest;

namespace MIDASS.Infrastructure.Authentication;

public class ApplicationAuthentication : BaseAuthentication, IApplicationAuthentication
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public ApplicationAuthentication(IJwtTokenServices jwtTokenServices, ICryptoServiceFactory cryptoServiceFactory,
        IOptions<JwtTokenOptions> jwtTokenOptions, IUserRepository userRepository,
        IExecutionContext executionContext, IRoleRepository roleRepository) : base(jwtTokenServices, cryptoServiceFactory, jwtTokenOptions,
        userRepository, executionContext)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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
        return newUser;
    }

    private Task<User?> GetUserOfLoginRequestAsync(LoginRequest loginRequest)
    {
        if (!string.IsNullOrEmpty(loginRequest.Username))
        {
            return _userRepository.GetByUsernameAsync(loginRequest.Username, "Role");
        }

        throw new BadRequestException("Username or email should be provided");
    }
}