
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Infrastructure.ImportExport.Export.Excels;
using Quartz;

namespace MIDASM.Infrastructure.ScheduleJobs;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class ScanBookBorrowingDueDateJob(
    IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository,
    IExportFactory exportFactory,
    IEmailRecordRepository emailRecordRepository) 
    : IJob
{
    public static readonly JobKey Key = new (nameof(ScanBookBorrowingDueDateJob), "scanDbGroup");
    public async Task Execute(IJobExecutionContext context)
    {
        var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var query = bookBorrowingRequestDetailRepository.GetQueryable()
            .Where(bd => bd.DueDate == targetDate)
            .Select(bd => new
            {
                bd.BookBorrowingRequest.RequesterId,
                bd.BookBorrowingRequest.Requester.Email,
                bd.BookBorrowingRequest.Requester.FirstName,
                bd.BookBorrowingRequest.Requester.LastName,
                bd.Book.Title,
                bd.Book.Author,
                bd.DueDate
            })

            .GroupBy(x => new { x.RequesterId, x.Email, x.FirstName, x.LastName })
            .Select(g => new
            {
                Email = g.Key.Email,
                FullName = g.Key.FirstName + " " + g.Key.LastName,
                Books = g.Select(b => new ExportBookDueDate
                {
                    Title = b.Title,
                    Author = b.Author,
                    DueDate = b.DueDate.ToString()
                })
            });

        var data = await query.ToListAsync();

        foreach (var it in data)
        {
            var excelExportData = exportFactory.ExportFile(nameof(ExportToExcel), new ExportRequest<ExportBookDueDate>
            {
                DataExport = it.Books,
                Title = $"List book due date at {targetDate}",  
            });
            var emailRecord = new EmailRecord()
            {
                ToEmail = it.Email,
                MineType = excelExportData.ContentType,
                Title = "Books Due Date Reminder",
                Type = "BooksDueDateEmail",
                AttachFile = excelExportData.DataBytes,
                UserFullName = it.FullName
            };
            emailRecordRepository.Add(emailRecord);
        }
        await emailRecordRepository.SaveChangesAsync();
    }
}
