using Autofac;
using GymErp.Common;
using GymErp.Domain.Subscriptions.Infrastructure;

namespace GymErp.Tenant;

public class TenantModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TenantAccessor>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DefaultTenantHttpAccessor>()
            .As<ITenantHttpAccessor>()
            .InstancePerLifetimeScope();

        builder.RegisterType<PostgresTenantStringConnection>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<ConfigTenantLocator>()
            .As<ITenantLocator>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SubscriptionsDbContextFactory>()
            .As<IEfDbContextFactory<SubscriptionsDbContext>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<TenantAccessorRegisterMiddleware>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<TenantEfMiddleware>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
