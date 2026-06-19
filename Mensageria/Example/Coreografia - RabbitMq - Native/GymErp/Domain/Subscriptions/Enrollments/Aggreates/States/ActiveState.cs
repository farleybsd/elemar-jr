using CSharpFunctionalExtensions;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates.States;

public class ActiveState : IEnrollmentState
{
    public EState CurrentState => throw new NotImplementedException();

    public Result Activate(Enrollment enrollment)
    {
        return Result.Failure("Inscrição já está ativa");
    }

    public Result Cancel(Enrollment enrollment)
    {
        enrollment.ChangeState(EState.Canceled);
        return Result.Success();
    }

    public Result Suspend(Enrollment enrollment)
    {
        enrollment.ChangeState(EState.Suspended);
        return Result.Success();
    }
}
