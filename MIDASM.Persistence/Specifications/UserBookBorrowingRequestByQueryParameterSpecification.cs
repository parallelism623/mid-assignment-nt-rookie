
using MIDASM.Application.Commons.Models;
using MIDASM.Domain.Entities;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Specifications;

public class UserBookBorrowingRequestByQueryParameterSpecification : Specification<BookBorrowingRequest, Guid>
{
    public UserBookBorrowingRequestByQueryParameterSpecification(Guid userId, QueryParameters queryParameters) : base(b => b.RequesterId == userId)
    {
        AddInclude(b => b.Approver!);
        AddInclude(b => b.BookBorrowingRequestDetails);
        AddOrderByDescending(b => b.CreatedAt);
    }
}
