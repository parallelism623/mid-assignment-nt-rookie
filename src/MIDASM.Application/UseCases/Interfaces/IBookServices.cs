using MIDASM.Application.Commons.Models.Books;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces;

public interface IBookServices
{
    Task<Result<PaginationResult<BookResponse>>> GetsAsync(BooksQueryParameters queryParameters);
    Task<Result<BookDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<List<BookDetailResponse>>> GetByIdsAsync(string ids);
    Task<Result<string>> CreateAsync(BookCreateRequest request);
    Task<Result<string>> UpdateAsync(BookUpdateRequest request);
    Task<Result<string>> DeleteAsync(Guid id);
}
