using Microsoft.OpenApi.Any;

namespace orders_api.Services
{
    public class ProductService: IProductService
    {
        public Task<IEnumerable<AnyType>> GetProductsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<AnyType> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<AnyType> CreateProductAsync(AnyType product)
        {
            throw new NotImplementedException();
        }
        public Task<AnyType> UpdateProductAsync(int id, AnyType product)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
