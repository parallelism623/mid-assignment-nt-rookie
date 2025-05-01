using MIDASM.Domain.Entities;

namespace MIDASM.Domain.Repositories;

public interface IUserRepository : IRepositoryBase<User, Guid>
{
    public Task<User?> GetByUsernameAsync(string userName, params string[] includes);
    public Task<User?> GetByEmailAsync(string email, params string[] includes);
}
