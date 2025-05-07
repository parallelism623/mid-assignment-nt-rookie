
namespace MIDASM.Domain.Abstract;

public interface ISoftDeleteEntity
{
    public bool IsDeleted { get; set; }
}
