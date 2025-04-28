
using MIDASS.Application.Commons.Models.Categories;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

public static class CategoryMapping
{
    public static CategoryResponse ToCategoryResponse(this Category category)
    {
        return new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            AvailableBooks = category.Books == null ? 0 : category.Books!.Sum(b => b.Available),
            QuantityBooks = category.Books == null ? 0 : category.Books!.Sum(b => b.Quantity)
        };
    }
}
