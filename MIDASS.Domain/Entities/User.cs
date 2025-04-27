
using MIDASS.Domain.Abstract;
using System.Text.Json.Serialization;

namespace MIDASS.Domain.Entities;

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
    public Guid RoleId { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; } = default!;
    [JsonIgnore]
    public ICollection<BookBorrowingRequest>? BookBorrowingRequests { get; set; }
    [JsonIgnore]
    public ICollection<BookBorrowingRequest>? BookBorrowingApproves { get; set; }

    public static User Create(string email, string username, string password, string firstName, string lastName, string? phoneNumber, Guid roleId)
    {
        return new()
        {
            Email = email,
            Username = username,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            RoleId = roleId,
            BookBorrowingLimit = 3,
            LastUpdateLimit = DateOnly.FromDateTime(DateTime.Now)
        };
    }

    
}
