using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Specifications;

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
    }
}