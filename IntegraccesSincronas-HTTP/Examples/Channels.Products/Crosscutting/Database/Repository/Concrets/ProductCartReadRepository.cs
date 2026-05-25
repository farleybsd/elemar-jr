using Channels.Products.Products;
using System.Collections.Concurrent;

namespace Channels.Products.Crosscutting.Database.Repository.Concrets;

public class ProductCartReadRepository : IProductCartReadRepository
{
    private readonly ConcurrentDictionary<Guid, ProductCart> _carts = new();
    public Task<List<ProductCart>> GetAllAsync()
    {
        return Task.FromResult(_carts.Values.ToList());
    }

    public Task<ProductCart?> GetByIdAsync(Guid id)
    {
        _carts.TryGetValue(id, out var cart);
        return Task.FromResult(cart);
    }
}
