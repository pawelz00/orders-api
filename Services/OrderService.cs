using orders_api.DTO.Order;

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

        public async Task<OrderResponse?> AddProductsToOrderAsync(int id, List<OrderItemCreate> requestData)
        {
            var order = await _repository.AddProductsToOrderAsync(id, requestData);

            return order;
        }

        public async Task<OrderResponse?> DeleteProductsFromOrderAsync(int id, List<int> productIds)
        {
            var order = await _repository.DeleteProductsFromOrderAsync(id, productIds);

            return order;
        }

        public async Task<OrderResponse?> CreateOrderAsync(OrderCreate order)
        {
            var createdOrder = await _repository.CreateOrderAsync(order);

            return createdOrder;
        }

        public Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate order)
        {
            var updatedOrder = _repository.UpdateOrderAsync(id, order);

            return updatedOrder;
        }
    }
}
