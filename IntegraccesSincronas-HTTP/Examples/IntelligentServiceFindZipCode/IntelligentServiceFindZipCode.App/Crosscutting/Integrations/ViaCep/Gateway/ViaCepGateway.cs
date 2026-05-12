namespace IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;

using IntelligentServiceFindZipCode.App.Crosscutting.Commom;
using IntelligentServiceFindZipCode.App.ZipCode.Endpoints;
using System.Runtime.ConstrainedExecution;

public readonly record struct ErrorResponse(
    bool Erro,
    string Mensagem);

public interface IViaCepGateway
{
    Task<Result<ZipCode>> GetZipCode(string cep);
}

internal class ViaCepGateway(HttpClient httpClient) : IViaCepGateway
{


    public async Task<Result<ZipCode>> GetZipCode(string cep)
    {
        var httpResponse = await httpClient.GetAsync($"{cep}/json/");

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error =
                await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            return Result<ZipCode>.Failure(error.Mensagem
                ?? "Erro desconhecido.");
        }

        ZipCode? response = await httpResponse.Content.ReadFromJsonAsync<ZipCode>();

        if (response is null)

            return Result<ZipCode>.Failure("ViaCep API returned empty response.");

        return Result<ZipCode>.Success(response.Value);
    }
}

