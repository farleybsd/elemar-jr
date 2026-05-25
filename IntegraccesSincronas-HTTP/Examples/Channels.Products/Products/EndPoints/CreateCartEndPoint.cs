using Channels.Products.Crosscutting;
using Channels.Products.Crosscutting.Services;
using Microsoft.AspNetCore.Mvc;

namespace Channels.Products.Products.EndPoints;

public record struct ProductCartRequest(string UserId, List<ProductCartItemRequest> ProductCartItems);
public record struct ProductCartItemRequest(Guid ProductId,string Name, int Quantity);

public record struct ProductCartResponse(Guid Id, string UserId, List<CartItemResponse> CartItems);
public record struct CartItemResponse(Guid Id, Guid ProductId, int Quantity, ProductResponse? Product);

public record struct ProductResponse(Guid Id, string Name, decimal Price);

public class CreateCartEndPoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("   ", CreateCartAsync)
          .WithTags("CreateCart")
          .WithName("CreateCart");
    }


    private static async Task<IResult> CreateCartAsync(
    ILogger<CreateCartEndPoint> logger,
    WriteBackCacheProductCartService service,
    [AsParameters] ProductCartRequest request,
    CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Create Cart EnPoint");

        var response = await service.AddAsync(request);

        return TypedResults.Created(
            $"/cart/{response.Id}",
            response
        );
    }
}
