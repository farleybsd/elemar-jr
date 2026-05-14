namespace IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;

using IntelligentServiceFindZipCode.App.Crosscutting.Commom;
using IntelligentServiceFindZipCode.App.ZipCode.Endpoints;
using Newtonsoft.Json;
using System.Runtime.ConstrainedExecution;

public readonly record struct ErrorResponse(
    bool Erro,
    string Mensagem);

public interface IViaCepGateway
{
    Task<Result<ZipCode>> GetZipCode(string cep, CancellationToken cancellationToken = default);
}

internal class ViaCepGateway(HttpClient httpClient) : IViaCepGateway
{


    public async Task<Result<ZipCode>> GetZipCode(string cep, CancellationToken cancellationToken = default)
    {
        var httpResponse = await httpClient.GetAsync($"{cep}/json/", cancellationToken).ConfigureAwait(false);
        await using var stream = await httpResponse
                                            .Content
                                            .ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content
                .ReadFromJsonAsync<ErrorResponse>(cancellationToken)
                .ConfigureAwait(false);

            return Result<ZipCode>.Failure(
                error.Mensagem ?? "Erro desconhecido.");
        }

        using var streamReader = new StreamReader(stream);

        using var jsonTextReader =
            new JsonTextReader(streamReader);

        var serializer = new JsonSerializer();

        ZipCode response =
            serializer.Deserialize<ZipCode>(jsonTextReader);

        return Result<ZipCode>.Success(response);
    }
}

