
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class UserByQueryParametersSpecification : Specification<User, Guid>
{
    public UserByQueryParametersSpecification(UserQueryParameters queryParameters) 
        : base(u => (string.IsNullOrEmpty(queryParameters.Search) 
                    || u.FirstName.Contains(queryParameters.Search) 
                    || u.LastName.Contains(queryParameters.Search)
                    || u.Email.Contains(queryParameters.Search)))
    {

    }
}
