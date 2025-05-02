
using MIDASM.Domain.Entities;
using System.Net.Mail;

namespace MIDASM.Domain.Repositories;

public interface IEmailRecordRepository : IRepositoryBase<EmailRecord, Guid>
{
}
