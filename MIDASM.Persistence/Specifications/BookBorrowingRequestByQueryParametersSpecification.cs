using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class BookBorrowingRequestByQueryParametersSpecification
    : Specification<BookBorrowingRequest, Guid>
{
    public BookBorrowingRequestByQueryParametersSpecification(BookBorrowingRequestQueryParameters queryParameters)
        : base(x => (queryParameters.GetStatus().Contains(x.Status)) 
                    && x.DateRequested >= queryParameters.FromRequestedDate
                    && x.DateRequested <= queryParameters.ToRequestedDate )
    {
        AddInclude(x => x.Approver!);
        AddInclude(x => x.BookBorrowingRequestDetails);
        AddInclude(x => x.Requester);
        AddOrderByDescending(x => x.DateRequested);
    }
}