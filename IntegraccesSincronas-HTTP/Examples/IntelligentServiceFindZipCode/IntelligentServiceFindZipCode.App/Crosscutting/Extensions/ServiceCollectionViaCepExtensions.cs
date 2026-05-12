using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep;
using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;
using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Validates;
using IntelligentServiceFindZipCode.App.Crosscutting.Logs;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.Fallback;
using System.Net;

namespace IntelligentServiceFindZipCode.App.Crosscutting.Extensions;

public static class ServiceCollectionViaCepExtensions
{
    public static IServiceCollection AddViaCep(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<LoggingHandlerViaCep>();

        services.AddSingleton<IValidateOptions<ViaCepSettings>, ViaCepSettingsValidate>();

        services.AddOptionsWithValidateOnStart<ViaCepSettings>()
            .Bind(configuration);

        services.AddTransient<ViaCepGateway>();

        services.Configure<ViaCepSettings>(
                 configuration.GetSection(ViaCepSettings.SectionName));

        services.AddHttpClient<IViaCepGateway, ViaCepGateway>((serviceProvider, httpClient) =>
        {
            var openAiSettings = serviceProvider.GetRequiredService<IOptions<ViaCepSettings>>();
            httpClient.BaseAddress = new Uri(openAiSettings.Value.BaseAddress);
            // httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiSettings.Value.ApiKey}");
        })
         .AddHttpMessageHandler<LoggingHandlerViaCep>()
         .AddResilienceHandler("viacep-resilience", pipeline =>
         {
             pipeline.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
             {
                 ShouldHandle = args =>
                     ValueTask.FromResult(args.Outcome.Exception is HttpRequestException),

                 OnFallback = args =>
                 {
                     Console.WriteLine("Fallback executado para ViaCep.");
                     return ValueTask.CompletedTask;
                 },

                 FallbackAction = _ =>
                 {
                     var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                     {
                         Content = JsonContent.Create(new
                         {
                             erro = true,
                             mensagem = "ViaCep indisponível no momento."
                         })
                     };

                     return Outcome.FromResultAsValueTask(response);
                 }
             });

             pipeline.AddRetry(new HttpRetryStrategyOptions
             {
                 MaxRetryAttempts = 3,
                 Delay = TimeSpan.FromSeconds(1),
                 BackoffType = DelayBackoffType.Exponential,
                 UseJitter = true,
                 OnRetry = args =>
                 {
                     Console.WriteLine($"Retry {args.AttemptNumber + 1} executado.");
                     return ValueTask.CompletedTask;
                 }
             });

             pipeline.AddTimeout(TimeSpan.FromSeconds(10));
         });

        return services;
    }

}
