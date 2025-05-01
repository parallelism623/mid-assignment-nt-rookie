
using MIDASS.Domain.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MIDASS.Domain.Entities;

public class Book : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string Author { get; set; } = default!;
    public int Quantity { get; set; }
    public int Available { get;set; }
    public Guid CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    [JsonIgnore]
    public Category Category { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public List<string>? SubImagesUrl { get; set; }
    [Timestamp]
    public byte[] TimeStamp { get; set; } = default!;
    public virtual ICollection<BookBorrowingRequestDetail>? BookBorrowingRequestDetails { get; set; }
    public virtual ICollection<BookReview>? BookReviews { get; set; }

    public static Book Create(string title, string description, string author, int quantity, int available, Guid categoryId, string? imageUrl, List<string>? subImagesUrl)
    {
        var book = new Book();
        book.Title = title;
        book.Description = description;
        book.Author = author;
        book.Available = available;
        book.Quantity = quantity;
        book.ImageUrl = imageUrl;
        book.SubImagesUrl = subImagesUrl;
        book.CategoryId = categoryId;
        return book;
    }

    public static void Update(Book book, string title, string description, string author, int addedQuantity, Guid categoryId)
    {
        book.Title = title;
        book.Description = description;
        book.Author = author;
        book.Quantity += addedQuantity;
        book.Available += addedQuantity;
        book.CategoryId = categoryId;
    }

    public static void UpdateImageUrl(Book book, string imageUrl)
    {
        book.ImageUrl = imageUrl;
    }
    public static void UpdateSubImagesUrl(Book book, List<string> subImagesUrl)
    {
        book.SubImagesUrl = subImagesUrl;
    }
}
