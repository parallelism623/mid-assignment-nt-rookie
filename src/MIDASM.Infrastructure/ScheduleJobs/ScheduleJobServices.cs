using DocumentFormat.OpenXml.Spreadsheet;
using MIDASM.Application.Commons.Models.SchedulerJobs;
using MIDASM.Application.Services.ScheduleJobs;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using Newtonsoft.Json;
using Quartz;

namespace MIDASM.Infrastructure.ScheduleJobs;

public class ScheduleJobServices(ISchedulerFactory schedulerFactory, 
    IServiceProvider serviceProvider)
    : IScheduleJobServices
{
    public async Task<Result<string>> CreateCronTriggerJobAsync(CronTriggerCreateRequest cronTriggerCreateRequest,
        CancellationToken cancellationToken = default)
    {
        var scheduler = await GetSchedulerAsync(cancellationToken);
        var triggerKey = GetTriggerKey(cronTriggerCreateRequest.TriggerName, cronTriggerCreateRequest.TriggerGroup);
        var jobKey = GetJobKey(cronTriggerCreateRequest.JobName,  cronTriggerCreateRequest.JobGroup);
        var oldTrigger = await scheduler.GetTrigger(triggerKey, cancellationToken);

        if (oldTrigger != null)
        {
            return Result<string>.Failure(SchedulerErrors.TriggerAlreadyExists);
        }

        var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);

        if (jobDetail == null)
        {
            return Result<string>.Failure(SchedulerErrors.JobNotExists);
        }


        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithCronSchedule(cronTriggerCreateRequest.CronExpression);
        if (cronTriggerCreateRequest.StartAt != null)
        {
            triggerBuilder.StartAt(cronTriggerCreateRequest.StartAt! ?? DateTimeOffset.Now);
        }
        if (cronTriggerCreateRequest.EndAt != null)
        {
            triggerBuilder.EndAt(cronTriggerCreateRequest.EndAt! ?? DateTimeOffset.Now);
        }

        var trigger = triggerBuilder.Build();
        
        await scheduler.ScheduleJob(jobDetail!, trigger!, cancellationToken);

        return SchedulerCommandMessages.CreateTriggerSuccess;
    }

    public Task<Result<string>> UpdateCronTriggerJobAsync(CronTriggerUpdateRequest cronTriggerUpdateRequest,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<string>> RemoveTriggerJobAsync(TriggerJobDeleteRequest triggerJobDelete,
        CancellationToken cancellationToken = default)
    {
        var scheduler = await GetSchedulerAsync(cancellationToken);
        var jobKey = GetJobKey(triggerJobDelete.JobName, triggerJobDelete.JobGroup);
        var triggerKey = GetTriggerKey(triggerJobDelete.TriggerName, triggerJobDelete.TriggerGroup);


        var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);
        if (jobDetail == null)
        {
            return Result<string>.Failure(SchedulerErrors.JobNotExists);
        }

        var trigger = await scheduler.GetTrigger(triggerKey, cancellationToken);

        if (trigger == null)
        {
            return Result<string>.Failure(SchedulerErrors.TriggerBeNotRegistered);
        }

        bool removed = await scheduler.UnscheduleJob(triggerKey, cancellationToken);

        if (!removed)
        {
            return Result<string>.Failure(SchedulerErrors.TriggerDeleteFailure);
        }

        return SchedulerCommandMessages.DeleteTriggerSuccess;
    }

    public async Task<Result<JobDetailWithTriggerResponse>> GetConTriggerOfJobAsync(string jobName, string? jobGroup, CancellationToken cancellationToken = default)
    {
        var scheduler = await GetSchedulerAsync(cancellationToken);
        var jobKey = GetJobKey(jobName, jobGroup);
        var jobDetails = await scheduler.GetJobDetail(jobKey, cancellationToken);

        if (jobDetails == null)
        {
            return Result<JobDetailWithTriggerResponse>.Failure(SchedulerErrors.JobNotExists);
        }
        IReadOnlyCollection<ITrigger> triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);

        var response = new JobDetailWithTriggerResponse()
        {
            JobName = jobDetails.Key.Name,
            JobGroup = jobDetails.Key.Group,
            Description = jobDetails.Description,
            Durable = jobDetails.Durable,
            NonConcurrent = jobDetails.ConcurrentExecutionDisallowed,
            RequestsRecovery = jobDetails.RequestsRecovery,
            JobData = JsonConvert.SerializeObject(jobDetails.JobDataMap, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }),
            TriggerJobs = triggers.Select(tr =>
            {
                var triggerJobResponse = new TriggerJobResponse
                {
                    TriggerName = tr.Key.Name, 
                    TriggerGroup = tr.Key.Group,
                    Description = tr.Description
                };

                if (tr is ICronTrigger cronTrigger)
                {
                    triggerJobResponse.CronExpression = cronTrigger.CronExpressionString;
                    triggerJobResponse.TimeZoneID = cronTrigger.TimeZone.Id;
                }

                return triggerJobResponse;
            }).ToList()
        };

        return response;
    }



    private static JobKey GetJobKey(string jobName, string? jobGroup = default)
    {
        var jobKey = new JobKey(jobName);
        if (!string.IsNullOrEmpty(jobGroup))
        {
            jobKey.Group = jobGroup;
        }

        return jobKey;
    }

    private static TriggerKey GetTriggerKey(string triggerName, string? triggerGroup = default)
    {
        var triggerKey = new TriggerKey(triggerName);
        if (!string.IsNullOrEmpty(triggerGroup))
        {
            triggerKey.Group = triggerGroup;
        }

        return triggerKey;
    }

    private Task<IScheduler> GetSchedulerAsync(CancellationToken cancellationToken = default)
    {
        return schedulerFactory.GetScheduler(cancellationToken);
    }
}
