
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using Quartz;

namespace MIDASM.Application.Commons.Models.SchedulerJobs;

public class CronTriggerCreateRequest
{
    public string JobName { get; set; } = default!;
    public string JobGroup { get; set; } = default!;
    public string TriggerName { get; set; } = default!;
    public string TriggerGroup { get; set; } = default!;
    public string CronExpression { get; set; } = default!;
    public DateTimeOffset? StartAt { get; set; }  
    public DateTimeOffset? EndAt { get; set; }

    public string? TimeZone { get; set; } = default;
}
public class CronTriggerCreateRequestValidator
    : AbstractValidator<CronTriggerCreateRequest>
{
    public CronTriggerCreateRequestValidator()
    {
        RuleFor(x => x.JobName)
            .NotEmpty().WithMessage(SchedulerValidationMessages.JobNameRequired)
            .MaximumLength(150).WithMessage(SchedulerValidationMessages.JobNameRequiredLessThanMaxLength);

        RuleFor(x => x.JobGroup)
            .NotEmpty().WithMessage(SchedulerValidationMessages.JobGroupRequired)
            .MaximumLength(150).WithMessage(SchedulerValidationMessages.JobGroupRequiredLessThanMaxLength);

        RuleFor(x => x.TriggerName)
            .NotEmpty().WithMessage(SchedulerValidationMessages.TriggerNameRequired)
            .MaximumLength(150).WithMessage(SchedulerValidationMessages.TriggerNameRequiredLessThanMaxLength);
        

        RuleFor(x => x.TriggerGroup)
            .NotEmpty().WithMessage(SchedulerValidationMessages.TriggerGroupRequired)
            .MaximumLength(150).WithMessage(SchedulerValidationMessages.TriggerGroupRequiredLessThanMaxLength);

        RuleFor(x => x.CronExpression)
            .NotEmpty().WithMessage(SchedulerValidationMessages.CronExpressionRequired)
            .Must(CronExpression.IsValidExpression)
                .WithMessage(SchedulerValidationMessages.CronExpressionInvalid);

        RuleFor(x => x.TimeZone)
            .Cascade(CascadeMode.Stop)
            .Must(BeAValidTimeZone).When(x => !string.IsNullOrWhiteSpace(x.TimeZone))
            .WithMessage(timeZone => SchedulerValidationMessages.InvalidTimeZoneIdentifier)
            .When(x => !string.IsNullOrEmpty(x.TimeZone));

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.StartAt.HasValue && x.EndAt.HasValue)
                    return x.EndAt.Value > x.StartAt.Value;
                return true;
            })
            .WithMessage(SchedulerValidationMessages.StartAtMustLessThanOrEqualEndAt);
    }

    private static bool BeAValidTimeZone(string? timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId!);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
