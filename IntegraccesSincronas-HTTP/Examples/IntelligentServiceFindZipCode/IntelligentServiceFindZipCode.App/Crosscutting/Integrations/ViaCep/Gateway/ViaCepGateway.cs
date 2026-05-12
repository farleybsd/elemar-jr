namespace IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;

using IntelligentServiceFindZipCode.App.Crosscutting.Commom;
using IntelligentServiceFindZipCode.App.ZipCode.Endpoints;
using System.Runtime.ConstrainedExecution;

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
            return Result<ZipCode>.Failure($"Error calling ViaCep API. StatusCode: {(int)httpResponse.StatusCode}");

        ZipCode? response = await httpResponse.Content.ReadFromJsonAsync<ZipCode>();

        if (response is null)

            return Result<ZipCode>.Failure("ViaCep API returned empty response.");

        return Result<ZipCode>.Success(response.Value);
    }
}

