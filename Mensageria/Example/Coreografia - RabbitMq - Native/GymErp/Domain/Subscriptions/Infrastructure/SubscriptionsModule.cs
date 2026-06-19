
using Autofac;
using GymErp.Common;
using GymErp.Domain.Subscriptions.Enrollments;
using Microsoft.EntityFrameworkCore;
using Endpoint = GymErp.Domain.Subscriptions.Features.AddNewEnrollment.Endpoint;
using Handler = GymErp.Domain.Subscriptions.Features.AddNewEnrollment.Handler;

namespace GymErp.Domain.Subscriptions.Infrastructure;

public class SubscriptionsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<NullServiceBus>()
            .As<IServiceBus>()
            .SingleInstance();

        builder.Register(c =>
        {
            var configuration = c.Resolve<IConfiguration>();
            var serviceBus = c.Resolve<IServiceBus>();

            var databaseConnection = configuration
                .GetSection("DatabaseConnection")
                .Get<DatabaseConnectionSettings>()
                ?? throw new InvalidOperationException("DatabaseConnection não configurado.");

            var connectionString = PostgresConnectionStringBuilder.Build(databaseConnection);

            var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new SubscriptionsDbContext(options, serviceBus);
        })
        .AsSelf()
        .InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        builder.RegisterType<EnrollmentRepository>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<Handler>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<Endpoint>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}