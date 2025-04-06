using Microsoft.Extensions.Logging;
using OrdersApi.Application.DTO.Product;
using OrdersApi.Application.Interfaces.Services;
using OrdersApi.Domain.Models;
using OrdersApi.Domain.Interfaces.Repositories;

namespace OrdersApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductResponse> CreateProductAsync(ProductCreate productDto)
        {
            _logger.LogInformation("Creating new product: {ProductName}", productDto.Name);
            
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price
            };

            var createdProduct = await _productRepository.AddProductAsync(product);

            return MapProductToDto(createdProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);

            bool deleted = await _productRepository.DeleteProductAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found for deletion.", id);
            }
          
            _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
            
            return deleted;
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Fetching product with ID: {ProductId}", id);
            
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found.", id);
                return null;
            }
            
            return MapProductToDto(product);
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsAsync()
        {
            _logger.LogInformation("Fetching all products.");

            var products = await _productRepository.GetProductsAsync();
            
            return products.Select(MapProductToDto);
        }

        public async Task<ProductResponse?> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            _logger.LogInformation("Attempting to update product with ID: {ProductId}", id);

            var existingProduct = await _productRepository.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found for update.", id);
                return null;
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Price = (decimal)productDto.Price;

            var updatedProduct = await _productRepository.UpdateProductAsync(id, existingProduct); 

            if (updatedProduct == null)
            {
                _logger.LogError("Failed to update product with ID: {ProductId} in repository.", id);
                return null;
            }

            _logger.LogInformation("Successfully updated product with ID: {ProductId}", id);
            
            return MapProductToDto(updatedProduct);
        }

        private static ProductResponse MapProductToDto(Product product)
        {
            return new ProductResponse(product);
        }
    }
}