namespace MIDASM.Application.Commons.Models.Books;

public class BooksQueryParameters : QueryParameters
{
    public bool Availability { get; set; } = false;
    public List<Guid> Ids { get; set; } = new();
    public List<Guid>? CategoryIds { get; set; }

    public int Rating { get; set; } = 0;

}