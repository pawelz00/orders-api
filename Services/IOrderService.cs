using Microsoft.OpenApi.Any;

namespace orders_api.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<AnyType>> GetOrdersAsync();
        Task<AnyType> GetOrderByIdAsync(int id);
        Task<AnyType> CreateOrderAsync(AnyType product);
        Task<AnyType> UpdateOrderAsync(int id, AnyType product);
        Task<bool> DeleteOrderAsync(int id);
    }
}
