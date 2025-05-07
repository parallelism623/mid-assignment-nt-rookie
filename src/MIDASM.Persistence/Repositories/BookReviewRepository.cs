
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookReviewRepository(ApplicationDbContext context) 
    : RepositoryBase<BookReview, Guid>(context), IBookReviewRepository
{
}
