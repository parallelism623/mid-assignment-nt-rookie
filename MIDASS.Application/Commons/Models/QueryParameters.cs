namespace MIDASS.Application.Commons.Models;

public abstract class QueryParameters
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string Search { get; set; } = string.Empty;

}
