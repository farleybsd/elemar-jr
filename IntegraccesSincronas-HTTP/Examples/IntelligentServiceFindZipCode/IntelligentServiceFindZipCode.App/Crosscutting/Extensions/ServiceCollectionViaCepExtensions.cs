using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep;
using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;
using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Validates;
using IntelligentServiceFindZipCode.App.Crosscutting.Logs;
using Microsoft.Extensions.Options;

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
        .AddHttpMessageHandler<LoggingHandlerViaCep>();

        return services;
    }

}
