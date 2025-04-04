
using orders_api.DTO.Order;
using orders_api.DTO.Product;

namespace orders_api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _repository.DeleteOrderAsync(id);
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            var product = await _repository.GetOrderByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync()
        {
            return await _repository.GetOrdersAsync();
        }

        public async Task<OrderResponse?> AddProductsToOrder(int id, List<OrderItemCreate> requestData)
        {
            var order = await _repository.AddProductsToOrder(id, requestData);

            return order;
        }

        public Task<OrderResponse?> DeleteProductsFromOrder(int id, List<int> productIds)
        {
            var order = _repository.DeleteProductsFromOrder(id, productIds);

            return order;
        }
    }
}
