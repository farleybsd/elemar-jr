using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Formatting.Compact;
using Serilog.Sinks.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace GymErp.Bootstrap;

internal static class ServicesExtensions
{
    public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.ToString());
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );

            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Cdc Consumer",
                    Description = "Consumer consolidator worker.",
                    Version = "v1"
                }
            );
        });
        return services;
    }
    
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(
            o =>
                o.AddPolicy(
                    "default",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                )
        );
        return services;
    }

    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        return services;
    }

    public static IServiceCollection AddWorkersServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
       
        return services;
    }

    public static IServiceCollection AddLogs(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithOpenTelemetrySpanId()
            .Enrich.WithOpenTelemetryTraceId()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";
                options.IncludedData = IncludedData.MessageTemplateTextAttribute |
                                        IncludedData.TraceIdField |
                                        IncludedData.SpanIdField;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName
                };
            })
            .WriteTo.Console(new CompactJsonFormatter())
            .CreateLogger();
        services.AddSingleton(Log.Logger);
        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryCache>(
            o =>
            {
                return new MemoryCache(new MemoryCacheOptions());
            });
        //services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddTelemetry(
        this IServiceCollection serviceCollection,
        string serviceName,
        string serviceVersion,
        IConfiguration configuration)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var openTelemetrySection = configuration.GetSection("OpenTelemetry");
        var exporterType = openTelemetrySection["Type"];
        var endpointRaw = openTelemetrySection["Endpoint"];

        var useConsoleExporter =
            string.Equals(exporterType, "console", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(endpointRaw);

        var endpoint = useConsoleExporter ? null : new Uri(endpointRaw!);

        Action<ResourceBuilder> configureResource = r => r.AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion,
            serviceInstanceId: Environment.MachineName);

        serviceCollection
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .AddSource(serviceName)
                    .AddSource("Silverback.Integration.Produce")
                    .AddSource("Silverback.Integration.Consume")
                    .AddSource("Silverback.Integration.Sequence")
                    .AddSource("Silverback.Core.Subscribers.InvokeSubscriber")
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity?.SetTag("env", environmentName);
                        };
                        opts.RecordException = true;
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (useConsoleExporter)
                    tracing.AddConsoleExporter();
                else
                    tracing.AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = endpoint!;
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(serviceName)
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation();

                if (useConsoleExporter)
                    metrics.AddConsoleExporter();
                else
                    metrics.AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = endpoint!;
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        return serviceCollection;
    }
}