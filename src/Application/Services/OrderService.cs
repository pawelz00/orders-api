using Microsoft.Extensions.Logging;
using OrdersApi.Application.DTO.Order;
using OrdersApi.Application.DTO.Product;
using OrdersApi.Application.Interfaces.Services;
using OrdersApi.Domain.Models;
using OrdersApi.Domain.Interfaces.Repositories;

namespace OrdersApi.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderCreate orderDto)
        {
            _logger.LogInformation("Creating new order for customer: {CustomerName}", orderDto.CustomerName);

            var order = new Order
            {
                CustomerName = orderDto.CustomerName,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var itemDto in orderDto.Products)
            {
                var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    _logger.LogError("Product with ID {ProductId} not found during order creation.", itemDto.ProductId);
                    throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                });
            }

            if (!order.OrderItems.Any())
            {
                _logger.LogWarning("Attempted to create an order with no valid items for customer {CustomerName}.", orderDto.CustomerName);
                throw new InvalidOperationException("Order must contain at least one valid item!");
            }


            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            _logger.LogInformation("Successfully created order with ID {OrderId} for customer {CustomerName}", createdOrder.Id, createdOrder.CustomerName);

            var detailedOrder = await _orderRepository.GetOrderByIdAsync(createdOrder.Id);

            return MapOrderToDto(detailedOrder!);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            _logger.LogInformation("Attempting to delete order with ID: {OrderId}", id);
            bool deleted = await _orderRepository.DeleteOrderAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for deletion.", id);
            }
            _logger.LogInformation("Successfully deleted order with ID: {OrderId}", id);
            return deleted;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            _logger.LogInformation("Fetching order with ID: {OrderId}", id);
            
            var order = await _orderRepository.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found.", id);
                return null;
            }
            
            return MapOrderToDto(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync()
        {
            _logger.LogInformation("Fetching all orders.");
            
            var orders = await _orderRepository.GetOrdersAsync();
            return orders.Select(MapOrderToDto);
        }

        public async Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdate orderDto)
        {
            _logger.LogInformation("Attempting to update order with ID: {OrderId}", id);
            
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for update.", id);
                return null;
            }


            if(orderDto.CustomerName != null) order.CustomerName = orderDto.CustomerName;
            if(orderDto.ShippingAddress != null) order.ShippingAddress = orderDto.ShippingAddress;
            if(orderDto.Status != null) order.Status = orderDto.Status;
            if(orderDto.Products != null)
            {
                order.OrderItems = new List<OrderItem>();
                foreach (var itemDto in orderDto.Products)
                {
                    var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        _logger.LogError("Product with ID {ProductId} not found during order update.", itemDto.ProductId);
                        throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
                    }
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                    });
                }
            }

            var updatedOrder = await _orderRepository.UpdateOrderAsync(id, order);

            if (updatedOrder == null)
            {
                _logger.LogError("Failed to update order with ID: {OrderId} in repository.", id);
                return null;            
            }

            _logger.LogInformation("Successfully updated order with ID: {OrderId}", id);


            return MapOrderToDto(updatedOrder);
        }

        public async Task<OrderResponse?> AddItemsToOrderAsync(int id, List<OrderItemCreate> itemsDto)
        {
            _logger.LogInformation("Attempting to add items to order with ID: {OrderId}", id);

            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for adding items.", id);
                return null;
            }

            var updated = await _orderRepository.AddProductsToOrderAsync(id, itemsDto.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }));

            if (updated == null)
            {
                _logger.LogError("Failed to add items to order with ID: {OrderId} in repository.", id);
                return null;
            }

            _logger.LogInformation("Successfully added items to order with ID: {OrderId}", id);

            return updated != null ? MapOrderToDto(updated) : null;
        }

        public async Task<OrderResponse?> RemoveItemsFromOrderAsync(int id, List<int> productIds)
        {
            _logger.LogInformation("Attempting to remove items from order with ID: {OrderId}", id);
            
            var order = await _orderRepository.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for removing items.", id);
                return null;
            }
            
            var updated = await _orderRepository.DeleteProductsFromOrderAsync(id, productIds);
            
            if (updated == null)
            {
                _logger.LogError("Failed to remove items from order with ID: {OrderId} in repository.", id);
                return null;
            }
            
            _logger.LogInformation("Successfully removed items from order with ID: {OrderId}", id);
            return updated != null ? MapOrderToDto(updated) : null;
        }

        private static OrderResponse MapOrderToDto(Order order)

        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                Products = order.OrderItems?.Select(oi => new ProductResponseWithQuantity
                {
                    Product = new ProductResponse(oi.Product),
                    Quantity = oi.Quantity
                }).ToList() ?? new List<ProductResponseWithQuantity>(),
            };
        }
    }
}