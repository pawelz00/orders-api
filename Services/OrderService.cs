using Microsoft.OpenApi.Any;

namespace orders_api.Services
{
    public class OrderService : IOrderService
    {
        public Task<AnyType> CreateOrderAsync(AnyType product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AnyType> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AnyType>> GetOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AnyType> UpdateOrderAsync(int id, AnyType product)
        {
            throw new NotImplementedException();
        }
    }
}
