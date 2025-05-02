
using MIDASM.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace MIDASM.Domain.Entities;

public class AuditLog : IEntity<Guid>
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? EntityName { get; set; } = default!;
    public string? EntityId { get; set; } = default!;
    public DateTime TimeStamp { get; set; }
    public string? BrowserInfo { get; set; }
    public string? HttpMethod { get; set; }
    public string? Url { get; set; }

    public string? ServiceName { get; set; }
    public string? MethodName { get; set; }
    public string? Parameters { get; set; }

    public int Status { get; set; }
    public string? SuccessDescription { get; set; }
    public string? ErrorDescription { get; set; }
}



