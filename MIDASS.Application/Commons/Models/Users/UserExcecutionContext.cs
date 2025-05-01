
namespace MIDASS.Application.Commons.Models.Users;

public class UserExecutionContext
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public UserRoleExecutionContext? Role { get; set; }
    public int BookBorrowingLimit { get; set; }
    
}


public class UserRoleExecutionContext
{
    public string Name { get; set; } = default!;
}