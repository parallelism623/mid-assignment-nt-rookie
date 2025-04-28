using MIDASS.Application.Commons.Models.Authentication;
using MIDASS.Application.Commons.Models.Categories;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.Services.Authentication;

public interface IApplicationAuthentication : IBaseAuthentication
{
    public Task<Result<string>> ConfirmEmailAsync (EmailConfirmRequest emailConfirmRequest);
    public Task<Result<string>> RefreshEmailConfirmAsync(RefreshEmailConfirmTokenRequest refreshEmailConfirm);
}
