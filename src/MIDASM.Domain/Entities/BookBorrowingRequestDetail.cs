
using Microsoft.VisualBasic;
using MIDASM.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASM.Domain.Entities;

public class BookBorrowingRequestDetail : AuditableEntity, IEntity<Guid>, ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid BookBorrowingRequestId { get; set; }
    public Guid BookId { get; set; }

    public DateOnly DueDate { get; set; }
    public bool IsDeleted { get; set; }
    public string? Noted { get; set; }
    public int ExtendDueDateTimes { get; set; }
    public DateOnly? ExtendDueDate { get; set; } = default!;


    [JsonIgnore]
    public Book Book { get; set; } = default!;
    [JsonIgnore]
    public BookBorrowingRequest BookBorrowingRequest { get; set; } = default!;
    public static BookBorrowingRequestDetail Create(Guid bookId, DateOnly dueDate, string? noted)
    {
        return new() { BookId = bookId, DueDate = dueDate, Noted = noted };
    }


    public static BookBorrowingRequestDetail Copy(BookBorrowingRequestDetail other)
    {
        return new()
        {
            Id = other.Id,
            BookBorrowingRequestId = other.BookBorrowingRequestId,
            BookId = other.BookId,
            DueDate = other.DueDate,
            Noted = other.Noted,
            IsDeleted = other.IsDeleted,
            ExtendDueDate = other.ExtendDueDate,
            ExtendDueDateTimes = other.ExtendDueDateTimes,
            Book = other.Book,
            BookBorrowingRequest = other.BookBorrowingRequest
        };
    }

    public static void CreateExtendDueDate(BookBorrowingRequestDetail detail, DateOnly dueDateExtend)
    {
        detail.ExtendDueDate = dueDateExtend;
        detail.ExtendDueDateTimes += 1;
    }
}
