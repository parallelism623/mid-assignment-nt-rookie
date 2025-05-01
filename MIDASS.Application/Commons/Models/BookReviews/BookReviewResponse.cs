
using MIDASS.Application.Commons.Models.Books;
using MIDASS.Application.Commons.Models.Users;

namespace MIDASS.Application.Commons.Models.BookReviews;

public class BookReviewDetailResponse
{
    public Guid Id { get; set; }
    public BookResponse Book { get; set; } = default!;
    public UserDetailResponse Reviewer { get; set; } = default!;

    public string Content { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Rating { get; set; } = default!;
    public DateOnly DateReview { get; set; } = default!;
}

public class BookReviewResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Rating { get; set; } = default!;
    public DateOnly DateReview { get; set; } = default!;
}