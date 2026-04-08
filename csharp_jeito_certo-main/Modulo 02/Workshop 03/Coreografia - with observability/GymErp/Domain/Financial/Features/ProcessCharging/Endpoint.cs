using FastEndpoints;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Microsoft.Extensions.Logging;

namespace GymErp.Domain.Financial.Features.ProcessCharging;

public class Endpoint(Handler handler, ILogger<Endpoint> logger) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/payments/one-off");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        logger.LogInformation("ProcessCharging request received");

        var message = new EnrollmentCreatedEvent(
            EnrollmentId: Guid.NewGuid(),
            ClientId: "one-off-payment",
            RequestedAtUtc: DateTime.UtcNow);

        await handler.HandleAsync(message, ct);

        await SendNoContentAsync(ct);
        logger.LogInformation("ProcessCharging completed");
    }
}
