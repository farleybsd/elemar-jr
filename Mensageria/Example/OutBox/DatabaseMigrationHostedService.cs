using Microsoft.EntityFrameworkCore;
using OutBox.Data;

namespace OutBox;

public sealed class DatabaseMigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseMigrationHostedService> _logger;

    public DatabaseMigrationHostedService(
       IServiceScopeFactory scopeFactory,
       ILogger<DatabaseMigrationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aplicando migrations do banco de dados");

        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<OutboxDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        _logger.LogInformation("Migrations aplicadas com sucesso");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
