
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

public static class UserMapping
{
    public static UserDetailResponse ToUserDetailResponse(this User user)
    {
        return new()
        {
            Id = user.Id,
            LastName = user.LastName,
            FirstName = user.FirstName,
            Email = user.Email,
            BookBorrowingLimit = user.BookBorrowingLimit,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
        };
    }
}
