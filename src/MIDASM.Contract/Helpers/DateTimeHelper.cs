
namespace MIDASM.Contract.Helpers;

public static class DateTimeHelper
{
    public static string ToShortTime(this DateTime dateTime)
    {
        return dateTime.ToString("g");
    }
    public static string ToShortTime(this DateTime? dateTime)
    {
        if (dateTime == null)
        {
            return string.Empty;
        }
        return ((DateTime)dateTime).ToString("g");
    }

    public static DateTimeOffset ToDateTimeOffSet(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime);
    }
}
