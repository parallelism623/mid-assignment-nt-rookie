
using MIDASM.Contract.SharedKernel;

namespace MISARM.API.Tests.TestHelpers;

public static class ResultPaginationCreateHelper
{
    public static Result<PaginationResult<T>> CreateStubResult<T>()
    {
        return PaginationResult<T>.Create(0, new());
    }    
}
