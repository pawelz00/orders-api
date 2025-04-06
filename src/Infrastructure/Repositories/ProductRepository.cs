using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrdersApi.Application.Interfaces.Infrastructure;
using OrdersApi.Domain.Models;
using OrdersApi.Domain.Interfaces.Repositories;

namespace OrdersApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IApplicationDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added product with ID {ProductId}", product.Id);

            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion.", id);
                return false;
            }

            // Check if product is in any order items before deleting
            bool isInOrder = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (isInOrder)
            {
                _logger.LogWarning("Attempted to delete product with ID {ProductId} which is part of an order.", id);

                throw new InvalidOperationException($"Cannot delete product ID {id} as it is used in existing orders.");
            }


            _context.Products.Remove(product);

            int changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted product with ID {ProductId}. Changes saved: {ChangeCount}", id, changes);

            return changes > 0;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("Querying for product with ID {ProductId}", id);
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            _logger.LogDebug("Querying for all products.");
            return await _context.Products.AsNoTracking().ToListAsync(); // Use AsNoTracking for read-only queries
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            _logger.LogDebug("Checking existence for product with ID {ProductId}", id);
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("Mismatched ID during product update. Provided ID: {ProvidedId}, Product ID: {ProductId}", id, product.Id);
                return null; 
            }

            // Find the existing entity first to update it

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update.", id);
                return null; 
            }

            _context.Products.Entry(existingProduct).CurrentValues.SetValues(product);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated product with ID {ProductId}", id);
                return existingProduct; 
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency exception updating product with ID {ProductId}", id);

                if (!await ProductExistsAsync(id))
                {
                    _logger.LogWarning("Product with ID {ProductId} no longer exists after concurrency exception.", id);
                    return null;
                }

                else
                {
                    throw; 
                }
            }
        }
    }
}