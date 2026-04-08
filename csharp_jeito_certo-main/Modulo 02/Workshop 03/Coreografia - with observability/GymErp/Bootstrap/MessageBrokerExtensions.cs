using GymErp.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace GymErp.Bootstrap;

internal static class MessageBrokerExtensions
{
    /// <summary>
    /// Configures the message broker based on appsettings (key "MessageBroker": "Silverback").
    /// The broker extension registers itself in MessageBrokerRegistry; this method invokes the selected one.
    /// </summary>
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        // Ensure broker extension is loaded so its static constructor runs and it registers in the registry.
        // `typeof(T)` não dispara static ctor; aqui forçamos explicitamente.
        RuntimeHelpers.RunClassConstructor(typeof(SilverbackServiceExtensions).TypeHandle);

        var brokerName = configuration["MessageBroker"] ?? "Silverback";
        MessageBrokerRegistry.Invoke(brokerName, services, configuration);
        return services;
    }
}
