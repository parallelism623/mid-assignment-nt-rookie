using MIDASS.Application.Commons.Models.Categories;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Specifications;

public class CategoryByQueryParametersSpecification : Specification<Category, Guid>
{
    public CategoryByQueryParametersSpecification(CategoriesQueryParameters queryParameters) 
        : base(c => c.Name.Contains(queryParameters.Search) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(queryParameters.Search)))
    {
        AddInclude(c => c.Books!);
    }
}
