using GymErp.Common;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Polly;

namespace GymErp.Domain.Financial.Features.ProcessCharging;

public class Handler(ILogger<Handler> logger, IServiceBus serviceBus)
{
    private const string PaymentApiBaseUrl = "https://v9e0g.wiremockapi.cloud";

    public async Task HandleAsync(EnrollmentCreatedEvent message, CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "ProcessCharging: starting for enrollment.id {enrollment.id}",
            message.EnrollmentId);

        try
        {
            logger.LogInformation(
                "ProcessCharging: processing charging for enrollment.id {enrollment.id}",
                message.EnrollmentId);

            var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                return await PaymentApiBaseUrl
                    .AppendPathSegment("payments")
                    .PostAsync(cancellationToken: cancellationToken);
            });

            if (response.Outcome == OutcomeType.Failure)
                throw response.FinalException!;
            if (!response.Result.ResponseMessage.IsSuccessStatusCode)
                throw new InvalidOperationException("Falha ao processar cobrança.");

            logger.LogDebug(
                "ProcessCharging: payment provider returned status.code {status.code} for enrollment.id {enrollment.id}",
                (int)response.Result.StatusCode,
                message.EnrollmentId);

            logger.LogInformation(
                "ProcessCharging: charging processed successfully for enrollment.id {enrollment.id}",
                message.EnrollmentId);

            await serviceBus.PublishAsync(new ChargingProcessedEvent(message.EnrollmentId));
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "ProcessCharging: failed for enrollment.id {enrollment.id}",
                message.EnrollmentId);
            throw;
        }
    }
}

