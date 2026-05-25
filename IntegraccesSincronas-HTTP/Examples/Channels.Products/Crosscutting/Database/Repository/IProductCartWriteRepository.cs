using Channels.Products.Products;

namespace Channels.Products.Crosscutting.Database.Repository
{
    public interface IProductCartWriteRepository
    {
        Task AddAsync(ProductCart productCart);
        Task UpdateAsync(ProductCart productCart);
        Task DeleteAsync(Guid id);
    }
}
