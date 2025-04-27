
namespace MIDASS.Contract.SharedKernel;
public class PaginationResult<T>
{
    public PaginationResult(int pageSize, int pageIndex, int totalCount, List<T> data)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalCount = totalCount;
        Items = data;
    }
    public static PaginationResult<T> Create(int pageSize, int pageIndex, int totalCount, List<T> data)
    {
        return new PaginationResult<T>(pageSize, pageIndex, totalCount, data); 
    }
    public int PageSize { get; set; }

    public int PageIndex { get; set; }
    public int TotalCount { get; set; }

    public IEnumerable<T>? Items { get; set; }
}