
namespace MIDASM.Domain.Enums;

public enum AuditAction
{
    Create,
    Update,
    Delete,
    Login,
    Logout,
    ApproveRequest,
    RejectRequest,
    BorrowBook,
    ExtendBorrow,
    ReviewBook
}