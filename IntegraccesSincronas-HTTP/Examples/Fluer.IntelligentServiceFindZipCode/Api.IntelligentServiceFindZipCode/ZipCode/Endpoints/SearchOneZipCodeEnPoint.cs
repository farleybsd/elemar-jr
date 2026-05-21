using Api.IntelligentServiceFindZipCode.Crosscutting;
using Api.IntelligentServiceFindZipCode.Crosscutting.Services;
using System.Text.Json.Serialization;

namespace Api.IntelligentServiceFindZipCode.ZipCode.Endpoints;

public readonly record struct SearchOneZipCodeRequest(string cep);
public readonly record struct SearchOneZipCodeResponse([property: JsonPropertyName("cep")] string Cep, [property: JsonPropertyName("logradouro")] string Logradouro,
                                      [property: JsonPropertyName("complemento")] string Complemento, [property: JsonPropertyName("unidade")] string Unidade,
                                      [property: JsonPropertyName("bairro")] string Bairro, [property: JsonPropertyName("localidade")] string Localidade,
                                      [property: JsonPropertyName("uf")] string Uf, [property: JsonPropertyName("estado")] string Estado,
                                      [property: JsonPropertyName("regiao")] string Regiao, [property: JsonPropertyName("ibge")] string Ibge,
                                      [property: JsonPropertyName("gia")] string Gia, [property: JsonPropertyName("ddd")] string Ddd,
                                      [property: JsonPropertyName("siafi")] string Siafi
);
public class SearchOneZipCodeEnPoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("SearchOne-ZipCode", SearchOneZipCodeAsync)
           .WithTags("searchOne-zipCode")
           .WithName("Search One ZipCode");
    }

    private async static Task<IResult> SearchOneZipCodeAsync(
      ILogger<SearchOneZipCodeEnPoint> logger,
      IviaCepGateway viaCepGateway
      ,
      [AsParameters] SearchOneZipCodeRequest request,
      CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Search One ZipCode EnPoint");

        var response = await viaCepGateway.GetZipCode(request.cep, cancellationToken);

        if (response.IsFailure)
        {
            logger.LogWarning(
                "Error searching zip code. Error: {Error}",
                response.Error);

            return TypedResults.BadRequest(new
            {
                response.Error
            });
        }

        return TypedResults.Ok(response.Value);
    }
}