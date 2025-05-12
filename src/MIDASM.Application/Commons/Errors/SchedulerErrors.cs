using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Commons.Errors;


public static class SchedulerErrorMessages
{
    public static string TriggerAlreadyExists => "Trigger already exists";
    public static string JobNotExists => "Job does not exists";
    public static string JobBeNotRegistered => "Job is not registered in services collection";
    public static string TriggerBeNotRegistered => "Trigger is not registered in job store";

    public static string TriggerDeleteFailure => "Delete trigger failure";
    public static string TriggerNotFound => "Trigger not found";
}
public static class SchedulerErrors
{
    public static Error TriggerAlreadyExists => new(nameof(SchedulerErrorMessages.TriggerAlreadyExists),
        SchedulerErrorMessages.TriggerAlreadyExists);

    public static Error JobNotExists => new(nameof(SchedulerErrorMessages.JobNotExists),
        SchedulerErrorMessages.JobNotExists);
    public static Error JobBeNotRegistered => new(nameof(SchedulerErrorMessages.JobBeNotRegistered),
        SchedulerErrorMessages.JobBeNotRegistered);
    public static Error TriggerBeNotRegistered => new(nameof(SchedulerErrorMessages.TriggerBeNotRegistered),
        SchedulerErrorMessages.TriggerBeNotRegistered);
    public static Error TriggerDeleteFailure => new(nameof(SchedulerErrorMessages.TriggerDeleteFailure),
        SchedulerErrorMessages.TriggerDeleteFailure);

    public static Error TriggerNotFound =>
        new(nameof(TriggerNotFound), SchedulerErrorMessages.TriggerNotFound);
}
