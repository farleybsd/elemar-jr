using GymErp.Common;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;

namespace GymErp.Domain.Subscriptions.Features.SuspendEnrollment;

public class Handler(EnrollmentRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<bool> HandleAsync(SuspendEnrollmentCommand request)
    {
        // O CancellationToken de endpoint não está sendo repassado para o handler.
        // Como o handler é resolvido via DI (Autofac), não incluímos CancellationToken no construtor.
        var cancellationToken = CancellationToken.None;

        if (!request.IsValid())
            return false;

        var enrollment = await repository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return false;

        var result = enrollment.Suspend(request.SuspensionStartDate, request.SuspensionEndDate);
        if (result.IsFailure)
            return false;

        await repository.UpdateAsync(enrollment, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return true;
    }
} 