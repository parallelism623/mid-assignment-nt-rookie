
namespace MIDASM.Contract.Messages.Validations;

public static class SchedulerValidationMessages
{
    public static string JobNameRequired => "Job name is required";
    public static string JobGroupRequired => "Job group is required";
    public static string CronExpressionInvalid => "CronExpression is not a valid Quartz cron expression.";
    public static string JobNameRequiredLessThanMaxLength => "Job name must be at most 150 characters";
    public static string JobGroupRequiredLessThanMaxLength => "Job group must be at most 150 characters";
    public static string TriggerNameRequired => "Trigger name is required";
    public static string TriggerGroupRequired => "Trigger group is required";
    public static string TriggerNameRequiredLessThanMaxLength => "Trigger name must be at most 150 characters";
    public static string TriggerGroupRequiredLessThanMaxLength => "Trigger group must be at most 150 characters";
    public static string CronExpressionRequired => "Cron expression is required";
    public static string InvalidTimeZoneIdentifier => "Invalid time zone identifier";
    public static string StartAtMustLessThanOrEqualEndAt => "Start time must less than or equal end time";
}
