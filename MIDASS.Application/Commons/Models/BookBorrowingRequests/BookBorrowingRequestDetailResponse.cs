
using MIDASS.Application.Commons.Models.Books;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Domain.Enums;

namespace MIDASS.Application.Commons.Models.BookBorrowingRequests;

public class BookBorrowingRequestDetailResponse
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public List<BookBorrowingRequestDetailItemResponse>? Items { get; set; }
}

public class BookBorrowingRequestDetailItemResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;

    public BookCategoryResponse Category { get; set; } = default!;

    public DateOnly DueDate { get; set; } = default!;
    public string? Noted { get; set; } = default;
}
