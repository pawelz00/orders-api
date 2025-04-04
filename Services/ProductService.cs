using orders_api.DTO.Product;
using orders_api.Models;

namespace orders_api.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsAsync()
        {
            var products = await _repository.GetProductsAsync();

            var productResponses = products.Select(p => new ProductResponse(p));

            return productResponses;
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductResponse(product);
        }

        public async Task<ProductResponse> CreateProductAsync(Product product)
        {
            var productCreate = new ProductCreate
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
            };

            await _repository.CreateProductAsync(productCreate);

            return new ProductResponse(product);
        }
        public async Task<ProductResponse?> UpdateProductAsync(int id, ProductUpdateDto product)
        {


            var updatedProduct = await _repository.UpdateProductAsync(id, product);

            if (updatedProduct == null) return null;

            return new ProductResponse(updatedProduct);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repository.DeleteProductAsync(id);
        }
    }
}
