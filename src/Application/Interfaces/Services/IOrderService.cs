using OrdersApi.Application.DTO.Order;

namespace OrdersApi.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetOrdersAsync();
        Task<OrderResponse?> GetOrderByIdAsync(int id);
        Task<OrderResponse> CreateOrderAsync(OrderCreate orderDto);
        Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate orderDto);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderResponse?> AddItemsToOrderAsync(int id, List<OrderItemCreate> itemsDto);
        Task<OrderResponse?> RemoveItemsFromOrderAsync(int id, List<int> productIds);
    }
}