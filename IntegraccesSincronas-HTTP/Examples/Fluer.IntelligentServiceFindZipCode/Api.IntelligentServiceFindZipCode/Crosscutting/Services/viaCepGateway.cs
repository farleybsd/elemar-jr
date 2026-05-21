using Api.IntelligentServiceFindZipCode.ZipCode.Endpoints;
using Flurl.Http;
using IntelligentServiceFindZipCode.App.Crosscutting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace Api.IntelligentServiceFindZipCode.Crosscutting.Services;

public class ViaCepServiceOptions
{
    public static string SectionName => "ViaCepService";

    [Required]
    public string ApiUrl { get; init; } = string.Empty;

    public int DefaultTimeoutMs { get; init; } = 500;
};

public interface IviaCepGateway
{
    Task<Result<SearchOneZipCodeResponse>> GetZipCode(string cep, CancellationToken cancellationToken);
}
public class ViaCepGateway(
    ResiliencePipelineProvider<string> pipelineProvider,
    IOptionsMonitor<ViaCepServiceOptions> options,
    ILogger<ViaCepGateway> logger) : IviaCepGateway
{
    private readonly ResiliencePipeline<IFlurlResponse> _resiliencePipeline =
        pipelineProvider.GetPipeline<IFlurlResponse>(
            "IntelligentServiceFindZipCode-policies"
        );

    public async Task<Result<SearchOneZipCodeResponse>> GetZipCode(
        string cep,
        CancellationToken cancellationToken = default)
    {

        try
        {
            logger.LogInformation("Buscanco o Cep: com o valor {cep}", cep);

            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                return await options.CurrentValue.ApiUrl
                    .WithTimeout(TimeSpan.FromMilliseconds(options.CurrentValue.DefaultTimeoutMs))
                    .AppendPathSegment($"{cep}/json/")
                    .AllowAnyHttpStatus()
                    .GetAsync(cancellationToken: token);
            }, cancellationToken);

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await response.GetStringAsync();
                logger.LogWarning(
                "Erro ao consultar o CEP {Cep}. StatusCode: {StatusCode}. Response: {Response}",
                cep,
                response.StatusCode,
                responseContent
       );
                return Result<SearchOneZipCodeResponse>.Failure(responseContent);
            }

            var responseAsJson = await response.GetJsonAsync<SearchOneZipCodeResponse>();

            logger.LogInformation(
            "Resposta da API ViaCep para o CEP {Cep}: {@Response}",
            cep,
            responseAsJson
            );

            return Result<SearchOneZipCodeResponse>.Success(responseAsJson);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Erro ao buscar CEP");

            return Result<SearchOneZipCodeResponse>.Failure(
                "Serviço de CEP indisponível."
            );
        }

    }
}
