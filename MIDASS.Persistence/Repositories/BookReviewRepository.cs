
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class BookReviewRepository : RepositoryBase<BookReview, Guid>, IBookReviewRepository
{
    public BookReviewRepository(ApplicationDbContext context) : base(context)
    {
    }
}
