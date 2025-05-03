using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class AuditLogByQueryParametersSpecification : Specification<AuditLog, Guid>
{
    public AuditLogByQueryParametersSpecification(AuditLogQueryParameters queryParameters)
        : base(al => (string.IsNullOrEmpty(queryParameters.ServiceName) 
        || (!string.IsNullOrEmpty(al.ServiceName) && al.ServiceName.Contains(queryParameters.ServiceName))))
    {
        AddOrderByDescending(al => al.TimeStamp);
    }
}
