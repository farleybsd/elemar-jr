using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Microsoft.Extensions.Logging;
using GymErp.Common.Telemetry;
using System.Diagnostics;

namespace GymErp.Domain.Financial.Features.ProcessCharging.Consumers;

public class EnrollmentCreatedEventConsumer(
    Handler processChargingHandler,
    ILogger<EnrollmentCreatedEventConsumer> logger)
{
    public async Task HandleAsync(EnrollmentCreatedEvent message, CancellationToken cancellationToken)
    {
        GymErpTelemetry.IncrementNewEnrollment(stage: "financial", status: "attempt");
        using var activity = GymErpTelemetry.StartEnrollmentCreatedConsumerActivity("Financial");
        activity?.SetTag("stage", "financial");
        GymErpTelemetry.AddMessagingTags(activity, system: "kafka", destination: "enrollment-events", consumerGroup: "financial-module");

        GymErpTelemetry.AddCommonEnrollmentTags(activity, message.EnrollmentId, message.ClientId);

        var sw = Stopwatch.StartNew();

        logger.LogDebug(
            "Consumer(Financial): EnrollmentCreatedEvent received for enrollment.id {enrollment.id}",
            message.EnrollmentId);

        try
        {
            await processChargingHandler.HandleAsync(message, cancellationToken);

            GymErpTelemetry.IncrementNewEnrollment(stage: "financial", status: "success");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("financial", "success", sw.Elapsed.TotalMilliseconds);
            GymErpTelemetry.RecordNewEnrollmentJourneyDurationMs("financial", (DateTime.UtcNow - message.RequestedAtUtc).TotalMilliseconds);

            logger.LogInformation(
                "Consumer(Financial): ProcessCharging completed for enrollment.id {enrollment.id}",
                message.EnrollmentId);
        }
        catch (Exception ex)
        {
            GymErpTelemetry.IncrementNewEnrollment(stage: "financial", status: "failure");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("financial", "failure", sw.Elapsed.TotalMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogWarning(
                ex,
                "Consumer(Financial): ProcessCharging failed for enrollment.id {enrollment.id}",
                message.EnrollmentId);
            throw;
        }
    }
}
