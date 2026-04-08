using GymErp.Common;

namespace GymErp.Domain.Subscriptions.Infrastructure;

public class UnitOfWork(IEfDbContextAccessor<SubscriptionsDbContext> accessor) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var context = accessor.Get();

        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        var context = accessor.Get();

        await context.SaveChangesAsync(cancellationToken);
    }
}