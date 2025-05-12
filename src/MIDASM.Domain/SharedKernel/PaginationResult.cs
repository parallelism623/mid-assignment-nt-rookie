namespace MIDASM.Domain.SharedKernel;
public class PaginationResult<T>
{
    public PaginationResult(int pageSize, int pageIndex, int totalCount, List<T> data)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalCount = totalCount;
        Items = data;
    }
    public PaginationResult(int totalCount, List<T> data)
    {
        PageSize = 10;
        PageIndex = 1;
        TotalCount = totalCount;
        Items = data;
    }

    public static PaginationResult<T> Create(int pageSize, int pageIndex, int totalCount, List<T> data)
    {
        return new PaginationResult<T>(pageSize, pageIndex, totalCount, data); 
    }
    public static PaginationResult<T> Create(int totalCount, List<T> data)
    {
        return new PaginationResult<T>(totalCount, data);
    }
    public int PageSize { get; set; }

    public int PageIndex { get; set; }
    public int TotalCount { get; set; }

    public IEnumerable<T>? Items { get; set; }
}