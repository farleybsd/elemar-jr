using GymErp.Domain.Financial.Features.Payments.Application.ProcessCharging;
using GymErp.Domain.Financial.Features.Payments.Domain;
using GymErp.Domain.Financial.Features.Payments.Services;
using GymErp.Domain.Financial.Infrastructure.Gateways;
using GymErp.Domain.Financial.Infrastructure.Persistencia;
using GymErp.Domain.Subscriptions.Features.Enrollments.Application.AddNewEnrollment;
using GymErp.Domain.Subscriptions.Features.Enrollments.Domain;
using GymErp.Domain.Subscriptions.Infrastructure;
using GymErp.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Silverback.Messaging.Broker;
using Xunit;
using FluentAssertions;
using FinancialHandler = GymErp.Domain.Financial.Features.Payments.Application.ProcessCharging.Handler;
using SubscriptionsHandler = GymErp.Domain.Subscriptions.Features.Enrollments.Application.AddNewEnrollment.Handler;

namespace GymErp.IntegrationTests.Financial.ProcessCharging;

public class MessagingFlowTests : IntegrationTestBase
{
    private FinancialHandler _financialHandler = null!;
    private SubscriptionsHandler _subscriptionsHandler = null!;
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
        var financialUnitOfWork = new FinancialUnitOfWork(financialDbContext);
        var chargeSemanticService = new PaymentChargeSemanticService(new FakePaymentProviderClient());
        var financialHandler = new FinancialHandler(paymentRepository, financialUnitOfWork, chargeSemanticService);

        var dbContextAccessor = new EfDbContextAccessor<SubscriptionsDbContext>(_dbContext);
        var enrollmentRepository = new EnrollmentRepository(dbContextAccessor);
        var unitOfWork = new UnitOfWork(_dbContext);
        var subscriptionsHandler = new SubscriptionsHandler(
            enrollmentRepository,
            unitOfWork,
            CancellationToken.None
        );

        _serviceProvider.GetService<IServiceCollection>()?.AddSingleton(financialHandler);
        _serviceProvider.GetService<IServiceCollection>()?.AddSingleton(subscriptionsHandler);

        _financialHandler = financialHandler;
        _subscriptionsHandler = subscriptionsHandler;
        _broker = _serviceProvider.GetRequiredService<IBroker>();
    }

    [Fact]
    public async Task CreateEnrollment_ShouldTriggerFinancialProcessing_WhenEnrollmentCreated()
    {
        // Arrange
        var request = new Request
        {
            Name = "João da Silva Santos",
            Email = "joao.silva@email.com",
            Phone = "11999999999",
            Document = "52998224725", // CPF válido
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "M",
            Address = "Rua Exemplo, 123"
        };

        // Act
        var result = await _subscriptionsHandler.HandleAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verificar se o evento foi publicado
        await VerifyMessagePublishedInKafkaTopic<EnrollmentCreatedEvent>(
            "enrollment-events", 
            1);
    }

    [Fact]
    public async Task FinancialHandler_ShouldProcessProcessChargingCommand_WhenCommandIsValid()
    {
        // Arrange
        var enrollmentId = Guid.NewGuid();
        var command = new ProcessChargingCommand(enrollmentId, 100m, "BRL");

        // Act
        var result = await _financialHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
