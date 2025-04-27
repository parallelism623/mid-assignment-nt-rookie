
namespace MIDASS.Application.Commons.Models.Users;

public class UserDetailResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default!;
    public int BookBorrowingLimit { get; set; }
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; } = default;

}

