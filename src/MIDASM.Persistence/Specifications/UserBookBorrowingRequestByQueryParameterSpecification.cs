
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Domain.Entities;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Specifications;

public class UserBookBorrowingRequestByQueryParameterSpecification : Specification<BookBorrowingRequest, Guid>
{
    public UserBookBorrowingRequestByQueryParameterSpecification(Guid userId, UserBookBorrowingRequestQueryParameters queryParameters)
        : base(b => b.RequesterId == userId && queryParameters.GetStatus().Contains(b.Status)
                    && b.DateRequested >= queryParameters.FromRequestedDate
                    && b.DateRequested <= queryParameters.ToRequestedDate )
    {
        AddInclude(b => b.Approver!);
        AddInclude(b => b.BookBorrowingRequestDetails);
        AddOrderByDescending(b => b.CreatedAt);
    }
}
