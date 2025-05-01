using MIDASM.Application.Commons.Models.Users;

namespace MIDASM.Application.Services.Authentication;

public interface IExecutionContext
{
    public void SetUser(UserExecutionContext user);
    public string GetRole();
    public string GetUserName();
    public string GetUserMail();
    public Guid GetUserId();
    public Guid GetJti();
    public void SetJti(Guid jti);

    public string GetAccessToken();
    public void SetAccessToken(string token);

}
