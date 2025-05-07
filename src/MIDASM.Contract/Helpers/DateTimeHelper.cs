
namespace MIDASM.Contract.Helpers;

public static class DateTimeHelper
{
    public static string DateTimeToShortTime(this DateTime dateTime)
    {
        return dateTime.ToString("g");
    }
}
