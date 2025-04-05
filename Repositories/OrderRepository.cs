using Microsoft.EntityFrameworkCore;
using orders_api.Data;
using orders_api.DTO.Product; 
using orders_api.DTO.Order;

public interface IOrderRepository
{
    Task<IEnumerable<OrderResponse>> GetOrdersAsync();
    Task<OrderResponse?> GetOrderByIdAsync(int id);
    Task<bool> DeleteOrderAsync(int id);
    Task<OrderResponse?> AddProductsToOrderAsync(int id, List<OrderItemCreate> products);
    Task<OrderResponse?> DeleteProductsFromOrderAsync(int id, List<int> productIds);
    Task<OrderResponse?> CreateOrderAsync(OrderCreate order);
    Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate order);
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

        public async Task<OrderResponse?> CreateOrderAsync(OrderCreate order)
        {
            // Check if products exist in db
            var productIds = order.Products.Select(p => p.ProductId).ToList();
            var productsInDb = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new ProductResponse(p))
                .ToListAsync();

            if (productsInDb == null || productsInDb.Count != productIds.Count)
            {
                return null;
            }

            // Make sure that the quantities are valid (greater than 0)
            foreach (var product in order.Products)
            {
                if (product.Quantity <= 0)
                {
                    return null;
                }
            }

            // Create a new order
            var newOrder = new Models.Order
            {
                CustomerName = order.CustomerName,
                ShippingAddress = order.ShippingAddress,
                OrderDate = order.OrderDate,
                Status = order.Status
            };

            // Add products to the order
            foreach (var product in order.Products)
            {
                newOrder.OrderItems.Add(new Models.OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity
                });
            }

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            var createdOrder = await GetOrderByIdAsync(newOrder.Id);

            return createdOrder;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var deletedRows = await _context.Orders
                .Where(o => o.Id == id)
                .ExecuteDeleteAsync(); 

            return deletedRows > 0;
        }

        public async Task<OrderResponse?> AddProductsToOrderAsync(int orderId, List<OrderItemCreate> products)
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

             _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            var updatedOrder = await GetOrderByIdAsync(orderId);
            
            return updatedOrder;
        }

        public async Task<OrderResponse?> DeleteProductsFromOrderAsync(int id, List<int> productIds)
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
            
            var updatedOrder = await GetOrderByIdAsync(id);

            return updatedOrder;
        }

        public async Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate order)
        {
            var existingOrder = await _context.Orders.FindAsync(id);

            if (existingOrder == null)
            {
                return null;
            }

            // Check if products exist in db
            var productIds = order.Products?.Select(p => p.ProductId).ToList();
            if (productIds != null)
            {
                var productsInDb = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => new ProductResponse(p))
                    .ToListAsync();

                if (productsInDb == null || productsInDb.Count != productIds.Count)
                {
                    return null;
                }
            }

            if (order.CustomerName != null) existingOrder.CustomerName = order.CustomerName;
            if (order.ShippingAddress != null) existingOrder.ShippingAddress = order.ShippingAddress;
            if (order.Status != null) existingOrder.Status = order.Status;
            if (order.Products != null)
            {
                _context.RemoveRange(_context.OrderItems.Where(oi => oi.OrderId == id));

                foreach (var product in order.Products)
                {
                    existingOrder.OrderItems.Add(new Models.OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity
                    });
                }
            }

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            var updatedOrder = await GetOrderByIdAsync(id);

            return updatedOrder;
        }
    }
}
