namespace MIDASS.Application.Commons.Models.Books;

public class BooksQueryParameters : QueryParameters
{
    public bool Availability { get; set; } = false;
    public List<Guid> Ids { get; set; } = new();

}