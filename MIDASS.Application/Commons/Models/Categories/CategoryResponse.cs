using MIDASS.Application.Commons.Models.Books;

namespace MIDASS.Application.Commons.Models.Categories;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<BookResponse>? Books { get; set; }
}
