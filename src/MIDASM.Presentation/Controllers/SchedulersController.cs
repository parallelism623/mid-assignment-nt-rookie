using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.SchedulerJobs;
using MIDASM.Application.Services.ScheduleJobs;
using MIDASM.Presentation.Controllers;

namespace MIDASS.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SchedulersController(IScheduleJobServices scheduleJobServices) : ApiBaseController
{
    [HttpPost]
    [Route("cron-triggers")]
    public async Task<IActionResult> CreateCronTriggerAsync([FromBody] CronTriggerCreateRequest cronTriggerRequestCreate)
    {
        var result = await scheduleJobServices.CreateCronTriggerJobAsync(cronTriggerRequestCreate);

        return ProcessResult(result);
    }

    [HttpGet]
    [Route("jobs")]
    public async Task<IActionResult> GetJobDetailAsync([FromQuery] string jobName, [FromQuery] string? jobGroup)
    {
        var result = await scheduleJobServices.GetConTriggerOfJobAsync(jobName, jobGroup);

        return ProcessResult(result);
    }

    [HttpDelete]
    [Route("jobs/triggers")]
    public async Task<IActionResult> DeleteJobTrigger([FromBody] TriggerJobDeleteRequest triggerJobDeleteRequest)
    {
        var result = await scheduleJobServices.RemoveTriggerJobAsync(triggerJobDeleteRequest);

        return ProcessResult(result);
    }

}
