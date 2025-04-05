using orders_api.DTO.Order;

namespace orders_api.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetOrdersAsync();
        Task<OrderResponse?> GetOrderByIdAsync(int id);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderResponse?> AddProductsToOrderAsync(int id, List<OrderItemCreate> products);
        Task<OrderResponse?> DeleteProductsFromOrderAsync(int id, List<int> productIds);
        Task<OrderResponse?> CreateOrderAsync(OrderCreate order);
        Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate order);
    }
}
