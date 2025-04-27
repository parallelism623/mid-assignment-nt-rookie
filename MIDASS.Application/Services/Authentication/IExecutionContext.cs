using MIDASS.Application.Commons.Models.Users;

namespace MIDASS.Application.Services.Authentication;

public interface IExecutionContext
{
    public void SetUser(UserExecutionContext user);
    public string GetRole();
    public string GetUserName();
    public Guid GetUserId();
    public Guid GetJti();
    public void SetJti(Guid jti);

    public string GetAccessToken();
    public void SetAccessToken(string token);

}
