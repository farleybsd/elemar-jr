using GymErp.Domain.Acesso.Features.AddPermissionToGreenList;
using GymErp.Common.Telemetry;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GymErp.Domain.Acesso.Features.AddPermissionToGreenList.Consumers;

public class EnrollmentCreatedEventConsumer(
    AddPermissionToGreenListHandler handler,
    ILogger<EnrollmentCreatedEventConsumer> logger)
{
    public async Task HandleAsync(EnrollmentCreatedEvent message, CancellationToken cancellationToken)
    {
        GymErpTelemetry.IncrementNewEnrollment(stage: "acesso", status: "attempt");
        using var activity = GymErpTelemetry.StartEnrollmentCreatedConsumerActivity("Acesso");
        activity?.SetTag("stage", "acesso");
        GymErpTelemetry.AddMessagingTags(activity, system: "kafka", destination: "enrollment-events", consumerGroup: "acesso-module");

        GymErpTelemetry.AddCommonEnrollmentTags(activity, message.EnrollmentId, message.ClientId);

        var sw = Stopwatch.StartNew();

        logger.LogDebug(
            "Consumer(Acesso): EnrollmentCreatedEvent received for enrollment.id {enrollment.id}",
            message.EnrollmentId);

        try
        {
            await handler.HandleAsync(message, cancellationToken);

            GymErpTelemetry.IncrementNewEnrollment(stage: "acesso", status: "success");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("acesso", "success", sw.Elapsed.TotalMilliseconds);
            GymErpTelemetry.RecordNewEnrollmentJourneyDurationMs("acesso", (DateTime.UtcNow - message.RequestedAtUtc).TotalMilliseconds);

            logger.LogInformation(
                "Consumer(Acesso): AddPermissionToGreenList completed for enrollment.id {enrollment.id}",
                message.EnrollmentId);
        }
        catch (Exception ex)
        {
            GymErpTelemetry.IncrementNewEnrollment(stage: "acesso", status: "failure");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("acesso", "failure", sw.Elapsed.TotalMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogWarning(
                ex,
                "Consumer(Acesso): AddPermissionToGreenList failed for enrollment.id {enrollment.id}",
                message.EnrollmentId);
            throw;
        }
    }
}

