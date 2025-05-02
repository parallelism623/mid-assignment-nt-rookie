
namespace MIDASM.Application.Commons.Models.Users;

public class UserQueryParameters : QueryParameters
{
    public string RoleIds { get; set; } = default!;

    public IEnumerable<Guid> GetRoleIds()
    {
        var roleIds = RoleIds?.Split(';').ToList();

        if(roleIds != null && roleIds.Any())
        {
            foreach (var roleId in roleIds)
            {
                if (!Guid.TryParse(roleId, out Guid id))
                {
                    continue;
                }
                yield return id;
            }
        }
    }
}
