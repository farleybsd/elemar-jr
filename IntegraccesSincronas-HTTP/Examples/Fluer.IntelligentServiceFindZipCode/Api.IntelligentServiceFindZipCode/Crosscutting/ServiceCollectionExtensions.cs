using Api.IntelligentServiceFindZipCode.Crosscutting.Filters;
using Api.IntelligentServiceFindZipCode.Crosscutting.Services;
using Asp.Versioning;
using Flurl.Http;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Serilog;
using System.Net;
using System.Reflection;
namespace Api.IntelligentServiceFindZipCode.Crosscutting;

public static class ServiceCollectionExtensions
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

    public static IServiceCollection AddIntelligentServiceFindZipCodeResiliencePolicies(this IServiceCollection services)
    {

        services.AddResiliencePipeline<string, IFlurlResponse>("IntelligentServiceFindZipCode-policies", pipeline =>
        {
            /*
             *   FALLBACK
             *   Executa quando tudo falha e devolve uma resposta padrão  para evitar quebrar a aplicação.
             */
            pipeline.AddFallback(new FallbackStrategyOptions<IFlurlResponse>
            {
                
                // Define em quais cenários o fallback será executado
                ShouldHandle = args =>
                   ValueTask.FromResult(
                       args.Outcome.Exception is FlurlHttpException || // Exceções HTTP lançadas pelo Flurl Exemplo: timeout, falha de DNS, conexão recusada, SSL etc.
                       args.Outcome.Exception is BrokenCircuitException || // Circuit Breaker aberto O Polly bloqueou novas chamadas porque muitas falhas aconteceram
                       args.Outcome.Result?.StatusCode == StatusCodes.Status503ServiceUnavailable  //HTTP 503 - Serviço indisponível  API externa temporariamente fora do ar ou sobrecarregada
                   ),

                // Executado no momento em que o fallback acontece
                OnFallback = args =>
                {
                    Log.Logger.Warning("Fallback executado para IntelligentServiceFindZipCode.");
                    return ValueTask.CompletedTask;
                },

                //  Ação executada quando o fallback é acionado  Neste caso, lançamos uma exceção customizada do Flurl  informando que o serviço está indisponível
                FallbackAction = args =>
                {
                    throw new FlurlHttpException(
                        null,
                        "IntelligentServiceFindZipCode indisponível no momento.",
                        null
                    );
                }
            
            });

            /*
             * RETRY
             * Tenta novamente automaticamente em falhas transitórias como timeout, 500, 503, gateway timeout etc.
             */
            pipeline.AddRetry(new RetryStrategyOptions<IFlurlResponse>
            {
                MaxRetryAttempts = 3,  // Quantidade máxima de novas tentativas
                Delay = TimeSpan.FromSeconds(1),// Tempo inicial entre cada retry
                BackoffType = DelayBackoffType.Exponential,  // Aumenta o delay progressivamente  Exemplo: 1s -> 2s -> 4s
                UseJitter = true, // Adiciona aleatoriedade no delay para evitar avalanche de requests

                
                // Define em quais cenários o retry será executado
                ShouldHandle = args =>
                    ValueTask.FromResult(
                        args.Outcome.Exception is FlurlHttpException || // Exceções HTTP lançadas pelo Flurl Exemplo: timeout, DNS, conexão recusada, SSL etc.
                        args.Outcome.Result?.StatusCode is 
                            StatusCodes.Status408RequestTimeout or //HTTP 408 - Timeout da requisição  O servidor demorou demais para responder
                            StatusCodes.Status429TooManyRequests or // HTTP 429 - Muitas requisições Rate limit atingido
                            StatusCodes.Status500InternalServerError or //HTTP 500 - Erro interno do servidor Falha inesperada na API externa
                            StatusCodes.Status502BadGateway or // HTTP 502 - Gateway inválido roblema entre proxy/load balancer e servidor
                            StatusCodes.Status503ServiceUnavailable or // HTTP 503 - Serviço indisponível A API externa está temporariamente fora do ar ou sobrecarregada
                            StatusCodes.Status504GatewayTimeout // HTTP 504 - Timeout entre gateways/proxies O servidor demorou demais para responder através do proxy/load balancer
                    ),

                // Executado toda vez que um retry acontecer
                OnRetry = args =>
                {
                    Log.Logger.Warning(
                        "Retry {AttemptNumber} executado para IntelligentServiceFindZipCode.",
                        args.AttemptNumber + 1
                    );

                    return ValueTask.CompletedTask;
                }
            });

            /*
             * CIRCUIT BREAKER
             * Abre o circuito quando muitas falhas acontecem seguidas, evitando sobrecarregar o serviço externo.
             * E fecha o circuito automaticamente depois de um tempo para testar se o serviço já se recuperou.
             */
            pipeline.AddCircuitBreaker(new CircuitBreakerStrategyOptions<IFlurlResponse>
            {
                FailureRatio = 0.7, // Percentual de falhas permitido antes de abrir o circuito  Exemplo: 0.7 = 70% das chamadas falharam
                SamplingDuration = TimeSpan.FromSeconds(30), // Janela de tempo utilizada para calcular as falhas  Exemplo: analisa os últimos 30 segundos
                MinimumThroughput = 10, // Quantidade mínima de requisições para começar a análise Evita abrir circuito com poucas chamadas
                BreakDuration = TimeSpan.FromSeconds(10),  // Tempo que o circuito ficará aberto sem permitir chamadas

                // Executado quando o circuito abre,  Novas chamadas serão bloqueadas
                OnOpened = args =>
                {
                    Log.Logger.Warning("Circuit Breaker do IntelligentServiceFindZipCode abriu.");
                    return ValueTask.CompletedTask;
                },

                // Executado quando o circuito entra em HALF-OPEN Permite algumas chamadas de teste
                OnHalfOpened = args =>
                {
                    Log.Logger.Information("Circuit Breaker do IntelligentServiceFindZipCode entrou em half-open.");
                    return ValueTask.CompletedTask;
                },

                // Executado quando o serviço volta a responder normalmente O circuito é fechado e libera novas chamadas
                OnClosed = args =>
                {
                    Log.Logger.Information("Circuit Breaker do IntelligentServiceFindZipCode fechou.");
                    return ValueTask.CompletedTask;
                }
            });

        });

        return services;
    }

    public static IServiceCollection AddViaCepServiceOptions(this IServiceCollection services)
    {
       
        services.AddOptions<ViaCepServiceOptions>()
            .BindConfiguration(ViaCepServiceOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IviaCepGateway, ViaCepGateway>();

        return services;
       
    }
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        return services;
    }

    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
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

}
