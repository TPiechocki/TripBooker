using Quartz;

namespace TripBooker.TourOperator.Infrastructure;

internal class UpdateJobTriggerListener : ITriggerListener
{
    private readonly ILogger<UpdateHotelsAndTransportsJob> _logger;

    public UpdateJobTriggerListener(ILogger<UpdateHotelsAndTransportsJob> logger)
    {
        _logger = logger;
    }

    public string Name => "UpdateJobTriggerListener";

    public Task TriggerComplete(
        ITrigger trigger,
        IJobExecutionContext context,
        SchedulerInstruction triggerInstructionCode,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggerFired(
        ITrigger trigger,
        IJobExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> VetoJobExecution(
        ITrigger trigger,
        IJobExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        if (trigger.JobKey.Name == nameof(UpdateHotelsAndTransportsJob)
            && File.Exists("StopUpdate"))
        {
            _logger.LogInformation("Update job stopped from execution");
            return Task.FromResult(true);
        }
        else
            return Task.FromResult(false);
    }
}
