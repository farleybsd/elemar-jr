using Channels.Products.Products;
using System.Collections.Concurrent;

namespace Channels.Products.Crosscutting.Database.Repository.Concrets;

public class ProductCartWriteRepository : IProductCartWriteRepository
{
    private readonly ConcurrentDictionary<Guid, ProductCart> _carts = new();
    public Task AddAsync(ProductCart productCart)
    {
        ArgumentNullException.ThrowIfNull(productCart);
        _carts[productCart.Id] = productCart;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O id do carrinho é inválido.", nameof(id));

        _carts.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ProductCart productCart)
    {
        ArgumentNullException.ThrowIfNull(productCart);

        _carts[productCart.Id] = productCart;
        return Task.CompletedTask;
    }
}
