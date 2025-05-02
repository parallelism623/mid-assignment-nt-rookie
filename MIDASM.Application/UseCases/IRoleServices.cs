
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.UseCases;

public interface IRoleServices
{
    Task<Result<List<RoleResponse>>> GetAsync();
}
