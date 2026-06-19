using CSharpFunctionalExtensions;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates.States;

public class SuspendedState : IEnrollmentState
{
    public EState CurrentState => throw new NotImplementedException();

    public Result Activate(Enrollment enrollment)
    {
        enrollment.ChangeState(EState.Active);
        return Result.Success();
    }

    public Result Cancel(Enrollment enrollment)
    {
        enrollment.ChangeState(EState.Canceled);
        return Result.Success();
    }

    public Result Suspend(Enrollment enrollment)
    {
        return Result.Failure("Inscrição já está suspensa");
    }
}
