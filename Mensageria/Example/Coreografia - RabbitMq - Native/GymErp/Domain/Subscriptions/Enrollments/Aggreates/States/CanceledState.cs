using CSharpFunctionalExtensions;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates.States;

public class CanceledState : IEnrollmentState
{
    public EState CurrentState => throw new NotImplementedException();

    public Result Activate(Enrollment enrollment)
    {
        return Result.Failure("Não é possível ativar uma inscrição cancelada");
    }

    public Result Cancel(Enrollment enrollment)
    {
        return Result.Failure("Inscrição já está cancelada");
    }

    public Result Suspend(Enrollment enrollment)
    {
        return Result.Failure("Não é possível suspender uma inscrição cancelada");
    }
}
