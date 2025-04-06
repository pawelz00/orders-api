using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrdersApi.Application.Interfaces.Infrastructure;
using OrdersApi.Domain.Models;
using OrdersApi.Domain.Interfaces.Repositories;

namespace OrdersApi.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IApplicationDbContext _context; 
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IApplicationDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IQueryable<Order> GetOrdersWithDetails()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking(); 
        }

        private IQueryable<Order> GetTrackedOrdersWithDetails()
        {
            return _context.Orders
               .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product);
        }


        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created order with ID {OrderId}", order.Id);

            return order; 
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id); 

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deletion.", id);
                return false;
            }

            _context.Orders.Remove(order);
            int changes = await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted order with ID {OrderId}. Changes saved: {ChangeCount}", id, changes);
            return changes > 0;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            _logger.LogDebug("Querying for order with ID {OrderId} with details.", id);

            return await GetTrackedOrdersWithDetails().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            _logger.LogDebug("Querying for all orders with details.");

            return await GetOrdersWithDetails().ToListAsync();
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            _logger.LogDebug("Checking existence for order with ID {OrderId}", id);

            return await _context.Orders.AnyAsync(e => e.Id == id);
        }


        public async Task<Order?> UpdateOrderAsync(int id, Order order)
        {
            if (id != order.Id)
            {
                _logger.LogWarning("Mismatched ID during order update. Provided ID: {ProvidedId}, Order ID: {OrderId}", id, order.Id);
                return null;
            }

            var existingOrder = await GetTrackedOrdersWithDetails().FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for update.", id);
                return null; 
            }

            _context.Orders.Entry(existingOrder).CurrentValues.SetValues(order);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated order with ID {OrderId}", id);
                return existingOrder;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency exception updating order with ID {OrderId}", id);

                if (!await OrderExistsAsync(id))
                {
                    _logger.LogWarning("Order with ID {OrderId} no longer exists after concurrency exception.", id);
                    return null;
                }

                else
                {
                    throw;
                }
            }

        }

        public async Task<Order?> AddProductsToOrderAsync(int orderId, IEnumerable<OrderItem> items)
        {
            var order = await GetTrackedOrdersWithDetails().FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for adding products.", orderId);
                return null;
            }

            foreach (var item in items)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity; 
                }
                else
                {
                    order.OrderItems.Add(item);
                }
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
           _logger.LogInformation("Added products to order with ID {OrderId}", orderId);
            return order;
        }

        public async Task<Order?> DeleteProductsFromOrderAsync(int orderId, IEnumerable<int> productIds)
        {
            var order = await GetTrackedOrdersWithDetails().FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deleting products.", orderId);
                return await Task.FromResult<Order?>(null);
            }

            foreach (var productId in productIds)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
                if (existingItem != null)
                {
                    order.OrderItems.Remove(existingItem);
                }
            }


            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted products from order with ID {OrderId}", orderId);
            return order;
        }
    }
}