
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Domain.Entities;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Specifications;

public class UserAuditLogByQueryParametersSpecification : Specification<AuditLog, Guid>
{
    public UserAuditLogByQueryParametersSpecification(Guid userId, UserAuditLogQueryParameters queryParameters) 
        : base(al => (al.UserId == userId)
        && al.EntityName == queryParameters.EntityName)
    {
        AddOrderByDescending(al => al.TimeStamp);
    }
}
