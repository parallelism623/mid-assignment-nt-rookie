
using MIDASM.Domain.Entities;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Specifications;

public class UserByQueryParametersSpecification : Specification<User, Guid>
{
    public UserByQueryParametersSpecification(Expression<Func<User, bool>>? criteria) : base(criteria)
    {
    }
}
