
using MIDASS.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASS.Domain.Entities;

public class Category : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    [JsonIgnore]
    public virtual ICollection<Book>? Books { get; set; }

}
