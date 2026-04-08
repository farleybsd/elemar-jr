using GymErp.Application.Ports.Inbound;
using GymErp.Application.UseCases.CancelEnrollment;
using GymErp.Domain.Enrollments;

namespace GymErp.Api.Consumers;

/// <summary>
/// Consumer Silverback que processa o evento <see cref="EnrollmentCanceledEvent"/>
/// e invoca a porta <see cref="ICancelEnrollmentUseCase"/>.
/// </summary>
public class EnrollmentCanceledEventConsumer(ICancelEnrollmentUseCase cancelEnrollmentUseCase)
{
    public async Task HandleAsync(EnrollmentCanceledEvent message, CancellationToken cancellationToken)
    {
        var request = new CancelEnrollmentRequest
        {
            EnrollmentId = message.EnrollmentId,
            Reason = "Cancelamento recebido via mensagem"
        };

        var result = await cancelEnrollmentUseCase.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error);
    }
}
