
using MIDASM.Application.Dispatcher.Commands;

namespace MIDASM.Application.AuditLogs.Commands.AddLog;

public class AddLogCommand : ICommand
{
    public string EntityId { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Dictionary<string, (string?, string?)>? ChangedProperties = default;
}
