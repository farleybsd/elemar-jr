using GymErp.Common;
using GymErp.Domain.Subscriptions.Enrollments.Aggreates;
using GymErp.Domain.Subscriptions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GymErp.Domain.Subscriptions.Enrollments;

public class EnrollmentRepository(SubscriptionsDbContext dbContext)
{
    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken)
    {
        await dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken)
    {
        dbContext.Enrollments.Update(enrollment);
        await Task.CompletedTask;
    }
}
