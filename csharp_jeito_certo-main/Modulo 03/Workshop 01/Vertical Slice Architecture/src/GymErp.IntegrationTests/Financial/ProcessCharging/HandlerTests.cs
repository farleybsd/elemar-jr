using GymErp.Domain.Financial.Features.Payments.Application.ProcessCharging;
using GymErp.Domain.Financial.Features.Payments.Domain;
using GymErp.Domain.Financial.Features.Payments.Services;
using GymErp.Domain.Financial.Infrastructure.Gateways;
using GymErp.Domain.Financial.Infrastructure.Persistencia;
using GymErp.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Silverback.Messaging.Broker;
using Xunit;
using FluentAssertions;
using FinancialHandler = GymErp.Domain.Financial.Features.Payments.Application.ProcessCharging.Handler;

namespace GymErp.IntegrationTests.Financial.ProcessCharging;

public class HandlerTests : IntegrationTestBase
{
    private FinancialHandler _handler = null!;
    private IBroker _broker = null!;

    protected override async Task SetupDatabase()
    {
        await base.SetupDatabase();

        var financialOptions = new DbContextOptionsBuilder<FinancialDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;
        var financialDbContext = new FinancialDbContext(financialOptions, _serviceBus);
        await financialDbContext.Database.EnsureCreatedAsync();

        var paymentRepository = new PaymentRepository(financialDbContext);
        var unitOfWork = new FinancialUnitOfWork(financialDbContext);
        var chargeSemanticService = new PaymentChargeSemanticService(new FakePaymentProviderClient());
        var handler = new FinancialHandler(paymentRepository, unitOfWork, chargeSemanticService);

        _serviceProvider.GetService<IServiceCollection>()?.AddSingleton(handler);

        _handler = handler;
        _broker = _serviceProvider.GetRequiredService<IBroker>();
    }

    [Fact]
    public async Task HandleAsync_ShouldProcessCharge_WhenProcessChargingCommandIsValid()
    {
        // Arrange
        var enrollmentId = Guid.NewGuid();
        var command = new ProcessChargingCommand(enrollmentId, 100m, "BRL");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
