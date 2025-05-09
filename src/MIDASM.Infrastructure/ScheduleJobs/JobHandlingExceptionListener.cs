
using Microsoft.Extensions.Logging;
using Quartz;

namespace MIDASM.Infrastructure.ScheduleJobs;

public class JobHandlingExceptionListener(ILogger<JobHandlingExceptionListener> logger) : IJobListener
{
    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var retryTimeKey = nameof(SchedulerJobConstant.MaxRetryTimes);
        var dataMap = context.JobDetail.JobDataMap;
        if (!dataMap.ContainsKey(retryTimeKey))
        {
            dataMap.Add(retryTimeKey, 0);
        }

        dataMap.Add(retryTimeKey, (int)dataMap[retryTimeKey] + 1);
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var retryTimeKey = nameof(SchedulerJobConstant.MaxRetryTimes);
        var dataMap = context.JobDetail.JobDataMap;
        if (jobException == null)
        {
            return;
        }

        var retryTimes = (int)dataMap[retryTimeKey];

        if (retryTimes > SchedulerJobConstant.MaxRetryTimes)
        {
            logger.LogError($"Retry job {nameof(context.JobDetail.Key.Name)} failure!");

            return;
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(context.JobDetail.Key.Name)
            .StartAt(DateTimeOffset.UtcNow.AddSeconds(SchedulerJobConstant.RetryTimeMinutes * retryTimes))
            .WithSimpleSchedule(s => s.WithRepeatCount(0))
            .Build();

        await context.Scheduler.ScheduleJob(trigger, cancellationToken);
    }

    public string Name => nameof(JobHandlingExceptionListener);
}
