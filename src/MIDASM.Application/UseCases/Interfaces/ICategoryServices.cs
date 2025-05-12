using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces
{
    public interface ICategoryServices
    {
        Task<Result<PaginationResult<CategoryResponse>>> GetCategoriesAsync();
        Task<Result<PaginationResult<CategoryResponse>>> GetCategoriesAsync(CategoriesQueryParameters queryParameters);
        Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id);
        Task<Result<string>> CreateCategoryAsync(CategoryCreateRequest createRequest);
        Task<Result<string>> UpdateCategoryAsync(CategoryUpdateRequest updateRequest);
        Task<Result<string>> DeleteCategoryAsync(Guid id);
    }
}
