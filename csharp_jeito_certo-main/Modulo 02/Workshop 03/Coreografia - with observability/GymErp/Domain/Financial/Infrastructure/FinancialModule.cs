using Autofac;
using ProcessChargingHandler = GymErp.Domain.Financial.Features.ProcessCharging.Handler;

namespace GymErp.Domain.Financial.Infrastructure;

public class FinancialModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ProcessChargingHandler>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
