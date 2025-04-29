
using MIDASS.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASS.Domain.Entities;

public class BookReview : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    [JsonIgnore]
    public User Reviewer { get; set; } = default!;
    public Guid BookId { get; set; }
    [JsonIgnore]
    public Book Book { get; set; } = default!;
    
    public int Rating { get; set; }
    public string? Content { get; set; } = default!;
    public string Title { get; set; } = default!;
    public bool IsDeleted { get; set; } 
}
