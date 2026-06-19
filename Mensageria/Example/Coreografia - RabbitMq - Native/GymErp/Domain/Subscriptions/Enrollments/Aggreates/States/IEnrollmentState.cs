using CSharpFunctionalExtensions;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates.States;

public interface IEnrollmentState
{
    EState CurrentState { get; }
    Result Activate(Enrollment enrollment);
    Result Suspend(Enrollment enrollment);
    Result Cancel(Enrollment enrollment);
}
