using MIDASM.Application.Commons.Models.Authentication;
using MIDASM.Application.UseCases.Interfaces;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Implements;

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
