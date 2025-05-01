
using MIDASS.Application.Commons.Models;
using MIDASS.Domain.Entities;
using System.Linq.Expressions;

namespace MIDASS.Persistence.Specifications;

public class UserBookBorrowingRequestByQueryParameterSpecification : Specification<BookBorrowingRequest, Guid>
{
    public UserBookBorrowingRequestByQueryParameterSpecification(Guid userId, QueryParameters queryParameters) : base(b => b.RequesterId == userId)
    {
        AddInclude(b => b.Approver!);
        AddInclude(b => b.BookBorrowingRequestDetails);
        AddOrderByDescending(b => b.CreatedAt);
    }
}
