
using MIDASM.Domain.Abstract;

namespace MIDASM.Domain.Entities;

public class Role : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public virtual ICollection<User>? Users { get; set; }
}
