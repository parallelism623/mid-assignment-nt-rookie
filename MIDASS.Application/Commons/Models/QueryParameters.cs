namespace MIDASS.Application.Commons.Models;

public class QueryParameters
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string Search { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 3;

}
