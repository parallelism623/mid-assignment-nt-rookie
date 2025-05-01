
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookReviewRepository : RepositoryBase<BookReview, Guid>, IBookReviewRepository
{
    public BookReviewRepository(ApplicationDbContext context) : base(context)
    {
    }
}
