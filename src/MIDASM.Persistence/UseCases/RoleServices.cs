
using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Services;

public class RoleServices(IRoleRepository roleRepository) : IRoleServices
{
    public async Task<Result<List<RoleResponse>>> GetAsync()
    {
        var roles = await roleRepository.GetAll();

        return roles.Select(r => new RoleResponse()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();
    }
}
