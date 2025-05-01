
using MIDASM.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASM.Domain.Entities;

public class Category : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    [JsonIgnore]
    public virtual ICollection<Book>? Books { get; set; }


    public static void Update(Category category, string name, string? description)
    {
        category.Name = name;
        category.Description = description;
    }
    public static Category Create(string name, string? description)
    {
        return new()
        {
            Name = name,
            Description = description
        };
    }
}
