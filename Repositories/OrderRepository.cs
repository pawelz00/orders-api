using Microsoft.EntityFrameworkCore;
using orders_api.Data;
using orders_api.DTO.Product; 
using orders_api.DTO.Order;

public interface IOrderRepository
{
    Task<IEnumerable<OrderResponse>> GetOrdersAsync();
    Task<OrderResponse?> GetOrderByIdAsync(int id);
    Task<bool> DeleteOrderAsync(int id);
    Task<OrderResponse?> AddProductsToOrder(int id, List<OrderItemCreate> products);
    Task<OrderResponse?> DeleteProductsFromOrder(int id, List<int> productIds);
}

namespace orders_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync()
        {
            var items = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderResponse
                {
                    CustomerName = o.CustomerName,
                    ShippingAddress = o.ShippingAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    products = o.OrderItems.Select(oi => new ProductResponseWithQuantity
                    {
                        Product = new ProductResponse(oi.Product),
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return items;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            var item = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == id)
                .Select(o => new OrderResponse
                {
                    CustomerName = o.CustomerName,
                    ShippingAddress = o.ShippingAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    products = o.OrderItems.Select(oi => new ProductResponseWithQuantity
                    {
                        Product = new ProductResponse(oi.Product),
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return item;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var deletedRows = await _context.Orders
                .Where(o => o.Id == id)
                .ExecuteDeleteAsync(); 

            return deletedRows > 0;
        }

        public async Task<OrderResponse?> AddProductsToOrder(int orderId, List<OrderItemCreate> products)
        {
            // Make sure that the product exists in db
            var productIds = products.Select(p => p.ProductId).ToList();
            var productsInDb = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new ProductResponse(p))
                .ToListAsync();

            if (productsInDb == null || productsInDb.Count() != productIds.Count())
            {
                return null;
            }

            // Check if order exists
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            // Check if products are already in order
            var existingProductIds = order.OrderItems.Select(oi => oi.ProductId).ToList();

            foreach (var product in products)
            {
                if (existingProductIds.Contains(product.ProductId))
                {
                    return null;
                }
            }


            // Add products to order
            foreach (var product in products)
            {
                order.OrderItems.Add(new Models.OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity
                });
            }

            // Save changes to the database
             _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            // Return the updated order
            var updatedOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == orderId)
                .Select(o => new OrderResponse
                {
                    CustomerName = o.CustomerName,
                    ShippingAddress = o.ShippingAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    products = o.OrderItems.Select(oi => new ProductResponseWithQuantity
                    {
                        Product = new ProductResponse(oi.Product),
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return updatedOrder;
        }

        public async Task<OrderResponse?> DeleteProductsFromOrder(int id, List<int> productIds)
        {
            // Check if order exists
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            // Check if products are in order
            var existingProductIds = order.OrderItems.Select(oi => oi.ProductId).ToList();
            foreach (var productId in productIds)
            {
                if (!existingProductIds.Contains(productId))
                {
                    return null;
                }
            }

            // Remove products from order
            foreach (var productId in productIds)
            {
                var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
                if (orderItem != null)
                {
                    _context.OrderItems.Remove(orderItem);
                }
            }

            await _context.SaveChangesAsync();
            
            var updatedOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == id)
                .Select(o => new OrderResponse
                {
                    CustomerName = o.CustomerName,
                    ShippingAddress = o.ShippingAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    products = o.OrderItems.Select(oi => new ProductResponseWithQuantity
                    {
                        Product = new ProductResponse(oi.Product),
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return updatedOrder;
        }
    }
}
