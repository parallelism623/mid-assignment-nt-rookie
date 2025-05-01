
namespace MIDASM.Domain.Abstract;
public abstract class AuditableEntity
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}