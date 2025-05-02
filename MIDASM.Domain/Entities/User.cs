
using MIDASM.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASM.Domain.Entities;

public class User : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default!;
    public bool IsDeleted { get; set; }
    public int BookBorrowingLimit { get; set; }
    public DateOnly LastUpdateLimit { get; set; } = DateOnly.MinValue;
    public string? RefreshToken { get; set; } = default;
    public DateTime RefreshTokenExpireTime { get; set; }
    public bool IsVerifyCode { get; set; } = false;
    public Guid RoleId { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; } = default!;
    [JsonIgnore]
    public ICollection<BookBorrowingRequest>? BookBorrowingRequests { get; set; }
    [JsonIgnore]
    public ICollection<BookBorrowingRequest>? BookBorrowingApproves { get; set; }

    [JsonIgnore]
    public ICollection<BookReview>? BookReviews { get; set; }

    public static User Create(string email, string username, string password, string firstName, 
        string lastName, string? phoneNumber, Guid roleId, bool isVerifyCode = false)
    {
        return new()
        {
            Email = email,
            Username = username,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            IsVerifyCode = isVerifyCode,
            RoleId = roleId,
            BookBorrowingLimit = 3,
            LastUpdateLimit = DateOnly.FromDateTime(DateTime.Now)
        };
    }

    public static void Update(User user, string email, string? password, string firstName, 
        string lastName, string? phoneNumber, int bookBorrowingLimit, Guid roleId)
    {
        user.Email = email;
        user.Password = !string.IsNullOrEmpty(password) ? password : user.Password;
        user.FirstName = firstName;
        user.PhoneNumber = phoneNumber;
        user.LastName = lastName;
        user.RoleId = roleId;
        user.BookBorrowingLimit = bookBorrowingLimit;
    }

    public static void Delete(User user)
    {
        user.IsDeleted = true;
    }
    
}
