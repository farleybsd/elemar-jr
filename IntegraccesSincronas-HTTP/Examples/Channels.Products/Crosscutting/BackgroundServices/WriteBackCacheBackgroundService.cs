using Channels.Products.Crosscutting.Database.Repository;
using Channels.Products.Crosscutting.Events;
using System.Threading.Channels;

namespace Channels.Products.Crosscutting.BackgroundServices;

public class WriteBackCacheBackgroundService(
 IServiceScopeFactory scopeFactory,
 Channel<ProductCartDispatchEvent> channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {

            await foreach (var command in channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = scopeFactory.CreateScope();
                var repositoryWrite = scope.ServiceProvider.GetRequiredService<IProductCartWriteRepository>();
                var repositoryRead = scope.ServiceProvider.GetRequiredService<IProductCartReadRepository>();

                var existingCart = await repositoryRead.GetByIdAsync(command.ProductCart.Id);
                if (existingCart is null)
                {
                    await repositoryWrite.AddAsync(command.ProductCart);
                    continue;
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
