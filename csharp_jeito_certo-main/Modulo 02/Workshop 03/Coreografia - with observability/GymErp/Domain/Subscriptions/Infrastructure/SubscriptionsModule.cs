using Autofac;
using GymErp.Common;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using GymErp.Domain.Subscriptions.Infrastructure;
using GymErp.Tenant;
using Endpoint = GymErp.Domain.Subscriptions.Features.AddNewEnrollment.Endpoint;
using Handler = GymErp.Domain.Subscriptions.Features.AddNewEnrollment.Handler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GymErp.Domain.Subscriptions.Infrastructure;

public class SubscriptionsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Contexto EF "do tenant" setado pelo middleware (TenantEFMiddleware).
        builder.RegisterType<EfDbContextAccessor<SubscriptionsDbContext>>()
            .As<IEfDbContextAccessor<SubscriptionsDbContext>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
        
        // Registra o DbContext
        builder.Register(ctx =>
            {
                var configuration = ctx.Resolve<IConfiguration>();
                var serviceBus = ctx.Resolve<IServiceBus>();

                // Por enquanto, criamos as options usando o "DatabaseConnection" base.
                // Em produção, o TenantEFMiddleware sobrescreveria esse DbContext por request.
                var postgresTenantStringConnection = new PostgresTenantStringConnection(configuration);
                var connectionString = postgresTenantStringConnection.GetTenantsConfigStringConnection();

                var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                return new SubscriptionsDbContext(options, serviceBus);
            })
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Repositório
        builder.RegisterType<EnrollmentRepository>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Handler de Nova Inscrição
        builder.RegisterType<Handler>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Endpoint de Nova Inscrição
        builder.RegisterType<Endpoint>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Handler de Suspensão
        builder.RegisterType<Features.SuspendEnrollment.Handler>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Endpoint de Suspensão
        builder.RegisterType<Features.SuspendEnrollment.Endpoint>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Handler de Cancelamento
        builder.RegisterType<Features.CancelEnrollment.Handler>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Registra o Endpoint de Cancelamento
        builder.RegisterType<Features.CancelEnrollment.Endpoint>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
} 