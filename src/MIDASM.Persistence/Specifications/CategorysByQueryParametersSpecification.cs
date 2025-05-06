using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class CategoryByQueryParametersSpecification : Specification<Category, Guid>
{
    public CategoryByQueryParametersSpecification(CategoriesQueryParameters queryParameters) 
        : base(c => (string.IsNullOrEmpty(queryParameters.Search) || (c.Name.Contains(queryParameters.Search) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(queryParameters.Search)))))
    {
        AddInclude(c => c.Books!);
        AddOrderByDescending(c => c.CreatedAt);
    }
}
