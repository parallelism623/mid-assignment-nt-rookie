using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MIDASS.Application.Commons.Options;
using MIDASS.Application.Services.Authentication;
using MIDASS.Contract.Constants;
using MIDASS.Domain.Entities;
using Rookies.Contract.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MIDASS.Infrastructure.Authentication;

public class JwtTokenServices : IJwtTokenServices
{
    private readonly IExecutionContext _executionContext;
    private readonly JwtTokenOptions _jwtOptions;
    private readonly IMemoryCache _memoryCache;


    public JwtTokenServices(IOptions<JwtTokenOptions> jwtOptions, IMemoryCache memoryCache,
        IExecutionContext executionContext)
    {
        _memoryCache = memoryCache;
        _executionContext = executionContext;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(_jwtOptions.PrivateKey), out _);
        RsaSecurityKey key = new(rsa);
        SigningCredentials credentials = new(key, SecurityAlgorithms.RsaSha256);

        ClaimsIdentity claimsList = new(new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, user.Role.Name),
        });

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = claimsList,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireTokenMinutes),           
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = credentials
        };
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public void RecallAccessToken()
    {
        var key = string.Format(CacheKey.RecallTokenKey, _executionContext.GetJti());
        _memoryCache.Set(key, nameof(CacheKey.RecallTokenKey),
            absoluteExpiration: DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpireTokenMinutes));
    }

    public ClaimsPrincipal ValidateAndDecode(string accessToken)
    {
        
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(_jwtOptions.PublicKey), out _);

        var key = new RsaSecurityKey(rsa);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = _jwtOptions.ValidateIssuer,
            ValidateAudience = _jwtOptions.ValidateAudience,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken validatedToken);
            if (validatedToken is JwtSecurityToken jwtToken && !jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            
            }
            return principal;
        }
        catch
        {
            throw new BadRequestException("Access token invalid");
        }
    }
}
