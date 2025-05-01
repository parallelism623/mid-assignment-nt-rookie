
namespace MIDASS.Application.Commons.Models.BookReviews;

public class BookReviewQueryParameters : QueryParameters
{
    public Guid? BookId { get; set; } = default;
    public Guid? ReviewId { get; set; } = default;
    public int[] Rating { get; set; } = [];

}
