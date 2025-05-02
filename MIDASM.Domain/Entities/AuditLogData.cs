
using System.ComponentModel.DataAnnotations;

namespace MIDASM.Domain.Entities;

public class AuditLogData
{
    public Guid Id { get; set; }
    public string? NewValue { get; set; }
    public string? OriginalValue { get; set; }
    public string? PropertyName { get; set; }
    public string? PropertyTypeFullName { get; set; }
    public Guid AuditLogId { get; set; }
}
