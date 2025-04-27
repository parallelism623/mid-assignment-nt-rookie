namespace MIDASS.Application.Commons.Models.Books;

public class BookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; } = default;
    public string Author { get; set; } = default!;
    public int Quantity { get; set; }
    public int Available { get; set; }
    public string? ImageUrl { get; set; }
    public BookCategoryResponse Category { get; set; } = default!;
}

public class BookDetailResponse : BookResponse
{
    public List<string>? SubImagesUrl { get; set; }
}

public class BookCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}