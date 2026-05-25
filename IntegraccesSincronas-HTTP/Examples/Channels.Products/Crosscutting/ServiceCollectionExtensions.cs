using Asp.Versioning;
using Channels.Products.Crosscutting.BackgroundServices;
using Channels.Products.Crosscutting.Database.Repository;
using Channels.Products.Crosscutting.Database.Repository.Concrets;
using Channels.Products.Crosscutting.Events;
using Channels.Products.Crosscutting.Filters;
using Channels.Products.Crosscutting.Services;
using System.Reflection;
using System.Threading.Channels;

namespace Channels.Products.Crosscutting;

public  static class ServiceCollectionExtensions
{
    public static void UseEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .ReportApiVersions()
            .Build();

        var globalGroup = app.MapGroup(prefix: string.Empty)
            .AddEndpointFilter<NormalizeBadRequestErrorsFilter>()
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1);

        var endpoints = Assembly.GetAssembly(typeof(Program))!
            .DefinedTypes
            .Where(type => type is { IsInterface: false, IsAbstract: false } && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => Activator.CreateInstance(type) as IEndpoint ?? throw new InvalidOperationException($"Could not create instance of IEndpoint {type.Name}"))
            .ToArray();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(globalGroup);
        }
    }

    public static IServiceCollection AddSwaggerWithVersioning(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    public static IServiceCollection AddBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService<WriteBackCacheBackgroundService>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<WriteBackCacheProductCartService>();
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddSingleton<IProductCartReadRepository, ProductCartReadRepository>();
        services.AddSingleton<IProductCartWriteRepository, ProductCartWriteRepository>();
        return services;
    }

    public static IServiceCollection AddChannels(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
            Channel.CreateBounded<ProductCartDispatchEvent>(
                new BoundedChannelOptions(100)
                {
                    FullMode = BoundedChannelFullMode.Wait
                }
            )
        );

        return services;
    }

}
