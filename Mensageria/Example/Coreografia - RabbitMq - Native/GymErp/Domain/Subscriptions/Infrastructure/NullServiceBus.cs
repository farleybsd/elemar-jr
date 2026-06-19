using GymErp.Common;

namespace GymErp.Domain.Subscriptions.Infrastructure
{
    public sealed class NullServiceBus : IServiceBus
    {
        public Task PublishAsync(object message)
        {
            return Task.CompletedTask;
        }
    }
}
