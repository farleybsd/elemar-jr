namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates.States;

public static class EnrollmentStateFactory
{
    public static IEnrollmentState CreateState(EState state)
    {
        return state switch
        {
            EState.Active => new ActiveState(),
            EState.Suspended => new SuspendedState(),
            EState.Canceled => new CanceledState(),
            _ => throw new ArgumentOutOfRangeException(nameof(state), $"Not expected enrollment state value: {state}")
        };
    }
}
