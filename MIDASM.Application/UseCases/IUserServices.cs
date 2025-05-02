using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.UseCases;

public interface IUserServices
{
    Task<Result<string>> CreateBookBorrowingRequestAsync(BookBorrowingRequestCreate bookBorrowingRequest);
    Task<Result<PaginationResult<BookBorrowingRequestResponse>>> GetBookBorrowingRequestByIdAsync(Guid id, UserBookBorrowingRequestQueryParameters queryParameters);
    Task<Result<UserDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<BookBorrowedRequestDetailResponse>>> GetBookBorrowedRequestDetailByIdAsync(Guid id, QueryParameters queryParameters);
    Task<Result<string>> ExtendDueDateBookBorrowed(DueDatedExtendRequest dueDatedExtendRequest);
    Task<Result<PaginationResult<UserDetailResponse>>>  GetAsync(UserQueryParameters queryParameters);
    Task<Result<string>> CreateAsync(UserCreateRequest createRequest);

    Task<Result<string>> UpdateAsync(UserUpdateRequest updateRequest);

    Task<Result<string>> DeleteAsync(Guid id);
}
