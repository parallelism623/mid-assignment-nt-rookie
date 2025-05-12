using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces;

public interface IRoleServices
{
    Task<Result<List<RoleResponse>>> GetAsync();
}
