using Asp.Versioning;
using IntelligentServiceFindZipCode.App.Crosscutting.Endpoint;
using IntelligentServiceFindZipCode.App.Crosscutting.Errors;
using System.Reflection;

namespace IntelligentServiceFindZipCode.App.Crosscutting.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static void UseEndPoints(this WebApplication app)
    {
        // Define as versões da API (v1 e v2) e habilita o retorno dessas versões nos headers
        var apiVersionSet = app.NewApiVersionSet()
           .HasApiVersion(new ApiVersion(1))
           .HasApiVersion(new ApiVersion(2))
           .ReportApiVersions()
           .Build();

        // Cria um grupo global de endpoints com prefixo versionado e aplica um filtro de erro padrão
        //Só intercepta respostas 400 já controladas
        var globalGroup = app.MapGroup(prefix: string.Empty)
            .AddEndpointFilter<NormalizeBadRequestErrorsFilter>()
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1);

        // Busca via reflection todas as classes concretas que implementam IEndpoint
        var endpoints = Assembly.GetAssembly(typeof(Program))!
            .DefinedTypes
            .Where(type => type is { IsInterface: false, IsAbstract: false }
                && type.IsAssignableTo(typeof(IEndpoint)))
            // Cria instâncias dinamicamente das classes encontradas
            .Select(type => Activator.CreateInstance(type) as IEndpoint
                ?? throw new InvalidOperationException($"Could not create instance of IEndpoint {type.Name}"))
            .ToArray();

        // Executa o método Map de cada endpoint, registrando as rotas no grupo global
        foreach (var endpoint in endpoints)
        {
            endpoint.Map(globalGroup);
        }
    }
}
