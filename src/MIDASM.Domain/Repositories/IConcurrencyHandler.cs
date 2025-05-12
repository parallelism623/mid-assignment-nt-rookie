
using MIDASM.Domain.Abstract;

namespace MIDASM.Domain.Repositories;

public interface IConcurrencyHandler<T>
{
    bool IsDbUpdateConcurrencyException(Exception ex);
    Task ApplyUpdatedValuesFromDataSource(Exception ex);
}
