using Microsoft.EntityFrameworkCore;
using orders_api.Data;
using orders_api.Models;
using orders_api.DTO.Product;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(ProductCreate product);
    Task<Product?> UpdateProductAsync(int id, ProductUpdateDto product);
    Task<bool> DeleteProductAsync(int id);
}

namespace orders_api.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProductAsync(ProductCreate product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product?> UpdateProductAsync(int id, ProductUpdateDto product)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if(existingProduct == null)
            {
                return null;
            }

            if (product.Name != null) existingProduct.Name = product.Name;
            if (product.Price != null) existingProduct.Price = (decimal)product.Price;
            if (product.Description != null) existingProduct.Description = product.Description;
            if (product.Category != null) existingProduct.Category = product.Category;

            _context.Products.Update(existingProduct);
            
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            var isProductInOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            
            if (isProductInOrders)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
