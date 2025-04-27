using MIDASS.Application.Commons.Models.Categories;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases
{
    public interface ICategoryServices
    {
        Task<Result<PaginationResult<CategoryResponse>>> GetCategoriesAsync(CategoriesQueryParameters queryParameters);
        Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id);
        Task<Result<string>> CreateCategoryAsync(CategoryCreateRequest createRequest);
        Task<Result<string>> UpdateCategoryAsync(CategoryUpdateRequest updateRequest);
        Task<Result<string>> DeleteCategoryAsync(Guid id);
    }
}
