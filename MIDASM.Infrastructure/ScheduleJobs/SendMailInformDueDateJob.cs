
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Services.Mail;
using MIDASM.Contract.Helpers;
using MIDASM.Domain.Repositories;
using Quartz;

namespace MIDASM.Infrastructure.ScheduleJobs;

public class SendMailInformDueDateJob(IMailServices mailServices, 
    IEmailRecordRepository mailRecordRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var mailRecords = await mailRecordRepository
            .GetQueryable()
            .Where(p => !p.Solved 
                        && p.CreatedAt.Day == DateTime.Now.Day).ToListAsync();
        var throttler = new SemaphoreSlim(10);
        mailRecords.ForEach(msg => msg.Solved = true);
        var sendMailTasks = mailRecords.Select(async msg =>
        {
            await throttler.WaitAsync();
            try
            {
                var content = await FileHelper.GetMailTemplateFile($"{msg.Type}.html");
                content = content
                    .Replace("{UserName}", msg.UserFullName)
                    .Replace("{DueDate}", DateTime.Today.AddDays(7).ToString("dd/MM/yyyy"));
                var sendMailData = new SendMailAttachmentData()
                {
                    Body = content,
                    IsBodyHtml = true,
                    FileBytes = msg.AttachFile,
                    ToEmail = msg.ToEmail,
                    Subject = msg.Title,
                    MineType = msg.MineType
                };
                await mailServices.SendMailWithAttachmentAsync(sendMailData);
            }
            catch           
            {
                throttler.Release();
                var ex = new InvalidOperationException(
                        $"Failed sending mail for record Id={msg.Id}, To={msg.ToEmail}");
                ex.Data["RecordId"] = msg.Id;
                throw ex;
            }
            finally { throttler.Release(); }

        }).ToArray();

        try
        {
            await Task.WhenAll(sendMailTasks);
        }
        catch
        {
            var errors = sendMailTasks
                .Where(t => t.IsFaulted)
                .SelectMany(t => t.Exception?.InnerExceptions ?? default!);

            if(errors.Any())
            {
                foreach (var ex in errors)
                {
                    if (ex != null && ex.Data.Contains("RecordId"))
                    {
                        var recordId = ex.Data["RecordId"]?.ToString();
                   
                        var mailRecord = mailRecords.FirstOrDefault(m => m.Id.ToString() == recordId);
                        if(mailRecord != null)
                        {
                            mailRecord.Solved = false;
                        }    
                    }
                }
            }    
        }
        mailRecordRepository.UpdateRange(mailRecords);
        await mailRecordRepository.SaveChangesAsync();
    }
}
