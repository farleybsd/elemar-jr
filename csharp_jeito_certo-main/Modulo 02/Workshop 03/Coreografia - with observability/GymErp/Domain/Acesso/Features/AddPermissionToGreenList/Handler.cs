using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Microsoft.Extensions.Logging;

namespace GymErp.Domain.Acesso.Features.AddPermissionToGreenList;

public class AddPermissionToGreenListHandler(ILogger<AddPermissionToGreenListHandler> logger)
{
    public Task HandleAsync(EnrollmentCreatedEvent message, CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "AddPermissionToGreenList: starting for enrollment.id {enrollment.id}",
            message.EnrollmentId);

        logger.LogInformation(
            "AddPermissionToGreenList operation executed successfully for enrollment.id {enrollment.id}",
            message.EnrollmentId);

        return Task.CompletedTask;
    }
}

