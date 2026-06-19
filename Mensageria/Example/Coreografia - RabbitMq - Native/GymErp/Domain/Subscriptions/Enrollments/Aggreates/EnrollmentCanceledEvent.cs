using GymErp.Common;

namespace GymErp.Domain.Subscriptions.Enrollments.Aggreates;

public record EnrollmentCanceledEvent(Guid EnrollmentId, DateTime CanceledAt) : IDomainEvent;
