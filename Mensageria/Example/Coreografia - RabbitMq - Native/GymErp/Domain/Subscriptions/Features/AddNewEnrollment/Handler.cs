using CSharpFunctionalExtensions;
using GymErp.Common;
using GymErp.Domain.Subscriptions.Enrollments;
using GymErp.Domain.Subscriptions.Enrollments.Aggreates;

namespace GymErp.Domain.Subscriptions.Features.AddNewEnrollment;

public class Handler(EnrollmentRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var enrollmentResult = Enrollment.Create(
            request.Name,
            request.Email,
            request.Phone,
            request.Document,
            request.BirthDate,
            request.Gender,
            request.Address
        );

        if (enrollmentResult.IsFailure)
            return Result.Failure<Guid>(enrollmentResult.Error);

        var enrollment = enrollmentResult.Value;

        await repository.AddAsync(enrollment, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return Result.Success(enrollment.Id);
    }
}