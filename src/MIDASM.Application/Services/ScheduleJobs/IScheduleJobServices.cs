
using MIDASM.Application.Commons.Models.SchedulerJobs;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.Services.ScheduleJobs;

public interface IScheduleJobServices
{
    Task<Result<string>> CreateCronTriggerJobAsync(CronTriggerCreateRequest cronTriggerCreateRequest, 
        CancellationToken cancellationToken = default);
    Task<Result<string>> UpdateCronTriggerJobAsync(CronTriggerUpdateRequest cronTriggerUpdateRequest, 
        CancellationToken cancellationToken = default);

    Task<Result<string>> RemoveTriggerJobAsync(TriggerJobDeleteRequest triggerJobDelete, CancellationToken cancellationToken = default);

    Task<Result<JobDetailWithTriggerResponse>> GetConTriggerOfJobAsync(string jobName, string? jobGroup, CancellationToken cancellationToken = default);
}
