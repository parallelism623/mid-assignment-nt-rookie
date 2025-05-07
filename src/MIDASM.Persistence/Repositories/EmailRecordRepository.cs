
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class EmailRecordRepository(ApplicationDbContext context)
    : RepositoryBase<EmailRecord, Guid>(context), IEmailRecordRepository
{
}
