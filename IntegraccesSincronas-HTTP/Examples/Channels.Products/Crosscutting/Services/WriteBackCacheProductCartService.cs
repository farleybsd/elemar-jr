using Channels.Products.Crosscutting.Database.Repository;
using Channels.Products.Crosscutting.Events;
using Channels.Products.Products;
using Channels.Products.Products.EndPoints;
using System.Threading.Channels;

namespace Channels.Products.Crosscutting.Services;

public class WriteBackCacheProductCartService
{
    private readonly IProductCartReadRepository _repository;
    private readonly Channel<ProductCartDispatchEvent> _channel;

    public WriteBackCacheProductCartService(IProductCartReadRepository repository, Channel<ProductCartDispatchEvent> channel)
    {
        _repository = repository;
        _channel = channel;
    }

    public async Task<ProductCartResponse> AddAsync(ProductCartRequest request)
    {

        var productCart = new ProductCart(
                            Guid.NewGuid(),
                            request.UserId
                                        );

        foreach (var item in request.ProductCartItems)
        {
            productCart.AddItem(
                productId: item.ProductId,
                name: item.Name,
                quantity: item.Quantity,
                price: Random.Shared.Next(100, 1000)
            );
        }

        // TODO: wrong ID here, repos has another id
        var cacheKey = $"productCart:{productCart.Id}";

        var productCartResponse = MapToProductCartResponse(productCart);

        //está escrevendo/enviando um evento dentro de um Channel.
        await _channel.Writer.WriteAsync(new ProductCartDispatchEvent(productCart));

        return productCartResponse;
    }

    private static ProductCartResponse MapToProductCartResponse(ProductCart cart)
    {
        return new ProductCartResponse(
            cart.Id,
            cart.UserId,
            cart.CartItems.Select(item => new CartItemResponse(
                item.Id,
                item.Id,
                item.Quantity,
                new ProductResponse(
                    item.Id,
                    item.Name,
                    item.Price)
            )).ToList()
        );
    }
}
