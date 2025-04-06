
using OrdersApi.Domain.Models;

namespace OrdersApi.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id); 
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(int id, Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<Order?> AddProductsToOrderAsync(int orderId, IEnumerable<OrderItem> items);
        Task<Order?> DeleteProductsFromOrderAsync(int orderId, IEnumerable<int> productIds);
        Task<bool> OrderExistsAsync(int id);

    }
}