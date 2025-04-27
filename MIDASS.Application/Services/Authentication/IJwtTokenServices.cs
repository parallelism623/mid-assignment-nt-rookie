using MIDASS.Domain.Entities;
using System.Security.Claims;

namespace MIDASS.Application.Services.Authentication;

public interface IJwtTokenServices
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    ClaimsPrincipal ValidateAndDecode(string accessToken);
    void RecallAccessToken();
}
