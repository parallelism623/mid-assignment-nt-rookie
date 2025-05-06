using MIDASM.Application.Commons.Models.Books;

namespace MIDASM.Application.Commons.Models.Categories;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<BookResponse>? Books { get; set; }
    public int AvailableBooks { get; set; }
    public int QuantityBooks { get; set; }
}
