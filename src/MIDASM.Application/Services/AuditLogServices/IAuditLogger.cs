using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Contract.SharedKernel;


namespace MIDASM.Application.Services.AuditLogServices;

public interface IAuditLogger
{
    Task LogAsync(string entityId, string entityName, string desciption, Dictionary<string, (string?, string?)>? changedProperties = default);
    Task<Result<PaginationResult<AuditLogResponse>>> GetActivitiesAsync(AuditLogQueryParameters queryParameters);
    Task<List<UserActiveDaysAuditLog>> GetUserActivitiesReportAsync(UserActivitiesQueryParameters userActivitiesQueryParameters);
    Task<Result<PaginationResult<UserAuditLogResponse>>> GetUserActivitiesAsync(Guid userId, UserAuditLogQueryParameters queryParameters);
}
