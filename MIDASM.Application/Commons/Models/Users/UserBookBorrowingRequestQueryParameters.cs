
using MIDASM.Domain.Enums;

namespace MIDASM.Application.Commons.Models.Users;

public class UserBookBorrowingRequestQueryParameters : QueryParameters
{
    public string Status { get; set; } = new('1', Enum.GetNames(typeof(BookBorrowingStatus)).Length);
    public DateOnly FromRequestedDate { get; set; } = DateOnly.MinValue;

    public DateOnly ToRequestedDate { get; set; } = DateOnly.MaxValue;
    public DateOnly FromApprovedDate { get; set; } = DateOnly.MinValue;

    public DateOnly ToApprovedDate { get; set; } = DateOnly.MaxValue;
    public IEnumerable<int> GetStatus()
    {
        for (int i = 0; i < Status.Length; i++)
        {
            if (Status[i] == '1')
                yield return i;
        }
    }
}
