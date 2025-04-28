
using Microsoft.VisualBasic;
using MIDASS.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASS.Domain.Entities;

public class BookBorrowingRequestDetail : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid BookBorrowingRequestId { get; set; }
    [JsonIgnore] 
    public BookBorrowingRequest BookBorrowingRequest { get; set; } = default!;
    public Guid BookId { get; set; }
    [JsonIgnore]
    public Book Book { get; set; } = default!;

    public DateOnly DueDate { get; set; }
    public bool IsExtend { get; set; }
    public bool IsDeleted { get; set; }
    public string? Noted { get; set; }
    public int ExtendDueDateTimes { get; set; }

    public static BookBorrowingRequestDetail Create(Guid bookId, DateOnly dueDate, string? noted)
    {
        return new() { BookId = bookId, DueDate = dueDate, Noted = noted };
    }
}
