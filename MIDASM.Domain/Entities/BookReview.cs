
using MIDASM.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASM.Domain.Entities;

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
    public DateOnly DateReview { get; set; } = default!;
    public bool IsDeleted { get; set; } 


    public static BookReview Create(Guid reviewerId, Guid bookId, string title, string? content, DateOnly dateReview, int rating)
    {
        return new()
        {
            ReviewerId = reviewerId,
            BookId = bookId,
            Title = title,
            Content = content,
            DateReview = dateReview,
            Rating = rating
        };
    }
}
