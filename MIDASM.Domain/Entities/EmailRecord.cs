
using MIDASM.Domain.Abstract;

namespace MIDASM.Domain.Entities;

public class EmailRecord : AuditableEntity, IEntity<Guid>
{
    public Guid Id {get;set;}
    public string ToEmail { get; set; } = default!;
    public string UserFullName { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool Solved { get; set; } = default!;

    public byte[]? AttachFile { get; set; } = default;

    public string MineType { get; set;}  = default!;
    
    
}
