using GymErp.Common;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates;

public record EnrollmentCreatedEvent : IDomainEvent
{
    public Guid EnrollmentId { get; init; }

    public EnrollmentCreatedEvent(Guid enrollmentId)
    {
        EnrollmentId = enrollmentId;
        Console.WriteLine($"Evento criado para EnrollmentId: {EnrollmentId}");
    }
}
