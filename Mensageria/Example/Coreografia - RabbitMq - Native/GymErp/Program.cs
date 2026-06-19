using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using GymErp.Common;

using GymErp.Domain.Subscriptions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    //container.RegisterModule(new CommonModule());
    container.RegisterModule(new SubscriptionsModule());
});

builder.Services
    .AddCors()
    .AddFastEndpoints(options =>
    {
        options.Assemblies = new[]
        {
            typeof(GymErp.Domain.Subscriptions.Features.AddNewEnrollment.Endpoint).Assembly
        };
    })
    .AddHttpContextAccessor()
    .AddOptions()
    .Configure<ServicesSettings>(builder.Configuration.GetSection("ServicesSettings"));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseFastEndpoints();

app.Run();