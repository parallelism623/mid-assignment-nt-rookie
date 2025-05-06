
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class EmailRecordRepository : RepositoryBase<EmailRecord, Guid>, IEmailRecordRepository
{
    public EmailRecordRepository(ApplicationDbContext context) : base(context)
    {
    }
}
