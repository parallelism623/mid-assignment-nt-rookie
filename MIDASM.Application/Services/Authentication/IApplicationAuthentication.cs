using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.Services.Authentication;

public interface IApplicationAuthentication : IBaseAuthentication
{
    public Task<Result<string>> ConfirmEmailAsync (EmailConfirmRequest emailConfirmRequest);
    public Task<Result<string>> RefreshEmailConfirmAsync(RefreshEmailConfirmTokenRequest refreshEmailConfirm);
}
