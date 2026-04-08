using Autofac;
using GymErp.Domain.Acesso.Features.AddPermissionToGreenList;

namespace GymErp.Domain.Acesso.Infrastructure;

public class AcessoModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AddPermissionToGreenListHandler>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}

