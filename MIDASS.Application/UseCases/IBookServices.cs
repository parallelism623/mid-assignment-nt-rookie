using MIDASS.Application.Commons.Models.Books;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases;

public interface IBookServices
{
    Task<Result<PaginationResult<BookResponse>>> GetsAsync(BooksQueryParameters queryParameters);
    Task<Result<BookDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<List<BookDetailResponse>>> GetByIdsAsync(string ids);
    Task<Result<string>> CreateAsync(BookCreateRequest request);
    Task<Result<string>> UpdateAsync(BookUpdateRequest request);
    Task<Result<string>> DeleteAsync(Guid id);
}
