using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Services.Mail;
using MIDASM.Contract.Helpers;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MimeKit;
using Quartz;
using System.Threading.Channels;

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
        int poolSize = 10;
        var channel = Channel.CreateUnbounded<MimeMessage>();
        
        _ = Task.Run(async () =>
        {
            mailRecords.ForEach(x => x.Solved = true);
            var mineMessages = await GetMineMessagesAsync(mailRecords);
            foreach (var msg in mineMessages)
                    await channel.Writer.WriteAsync(msg);
            channel.Writer.Complete();
        });

  
        var consumers = new List<Task>();
        for (int i = 0; i < poolSize; i++)
        {
            consumers.Add(Task.Run(async () =>
            {
                using var client = await mailServices.GetSmtpClient();
                await foreach (var message in channel.Reader.ReadAllAsync())
                {
                    await client.SendAsync(message);
                }

                await client.DisconnectAsync(true);
            }));
        }

        await Task.WhenAll(consumers);
        await mailRecordRepository.SaveChangesAsync();
    }



    private async Task<List<MimeMessage>> GetMineMessagesAsync(List<EmailRecord> mailRecords)
    {
        if (mailRecords == null || mailRecords.Count == 0)
        {
            return new();
        }
        var template = await FileHelper.GetMailTemplateFile($"{mailRecords[0].Type}.html");
        return mailRecords.Select( mailRecord => GetMineMessageAsync(mailRecord, template)).ToList();
    }
    private  MimeMessage GetMineMessageAsync(EmailRecord msg, string template)
    {
        var content = template;
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
            MineType = msg.MineType,
            FileName = $"book-due-date-list-at-{DateTime.UtcNow.AddDays(7):YYYY-MM-DD}"
        };
        return mailServices.GetMimeMessage(sendMailData);
    }
}
