using Channels.Products.Products;

namespace Channels.Products.Crosscutting.Database.Repository;

public interface IProductCartReadRepository
{
    Task<ProductCart?> GetByIdAsync(Guid id);
    Task<List<ProductCart>> GetAllAsync();
}
