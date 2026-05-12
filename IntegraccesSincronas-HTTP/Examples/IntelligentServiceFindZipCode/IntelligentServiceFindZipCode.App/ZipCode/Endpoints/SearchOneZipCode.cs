using IntelligentServiceFindZipCode.App.Crosscutting.Endpoint;
using IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Gateway;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentServiceFindZipCode.App.ZipCode.Endpoints;

public readonly record struct SearchOneZipCodeRequest(string cep);
public readonly record struct SearchOneZipCodeRequestResponse(string name, string Cpf, string Email, decimal walletBalance);
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
      IViaCepGateway viaCepGateway,
      [AsParameters] SearchOneZipCodeRequest request,
      CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Search One ZipCode EnPoint");

        var response = await viaCepGateway.GetZipCode(request.cep);

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
