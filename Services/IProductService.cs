using Microsoft.OpenApi.Any;

namespace orders_api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<AnyType>> GetProductsAsync();
        Task<AnyType> GetProductByIdAsync(int id);
        Task<AnyType> CreateProductAsync(AnyType product);
        Task<AnyType> UpdateProductAsync(int id, AnyType product);
        Task<bool> DeleteProductAsync(int id);
    }
}
