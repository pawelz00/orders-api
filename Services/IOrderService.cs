using orders_api.DTO.Order;

namespace orders_api.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetOrdersAsync();
        Task<OrderResponse?> GetOrderByIdAsync(int id);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderResponse?> AddProductsToOrder(int id, List<OrderItemCreate> products);
        Task<OrderResponse?> DeleteProductsFromOrder(int id, List<int> productIds);
    }
}
