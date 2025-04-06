using OrdersApi.Application.DTO.Product;

namespace OrdersApi.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(ProductCreate productDto);
        Task<ProductResponse?> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}