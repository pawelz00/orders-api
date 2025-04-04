using orders_api.DTO.Product;
using orders_api.Models;

namespace orders_api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(Product product);
        Task<ProductResponse?> UpdateProductAsync(int id, ProductUpdateDto product);
        Task<bool> DeleteProductAsync(int id);
    }
}
