using Microsoft.AspNetCore.Mvc;
using OrdersApi.Application.DTO.Order;
using OrdersApi.Application.Interfaces.Services; 

namespace OrdersApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            _logger.LogInformation("API Endpoint called: GetOrders");
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            _logger.LogInformation("API Endpoint called: GetOrder with ID {OrderId}", id);

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found by API.", id);
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostOrder([FromBody] OrderCreate orderDto)
        {
            _logger.LogInformation("API Endpoint called: PostOrder for Customer {CustomerName}", orderDto.CustomerName);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for PostOrder.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product not found during order creation via API.");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Attempt to create order with no valid items via API.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutOrder(int id, [FromBody] OrderUpdate orderDto)
        {
            _logger.LogInformation("API Endpoint called: PutOrder for ID {OrderId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for PutOrder ID {OrderId}.", id);
                return BadRequest(ModelState);
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);

            if (updatedOrder == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for update by API.", id);
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("API Endpoint called: DeleteOrder for ID {OrderId}", id);

            var deleted = await _orderService.DeleteOrderAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for delete by API.", id);

                return NotFound(new { message = $"Order with ID {id} not found." });
            }
            return NoContent();
        }

        [HttpPost("{id}/add-items")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItemsToOrder(int id, [FromBody] List<OrderItemCreate> orderItems)
        {
            _logger.LogInformation("API Endpoint called: AddItemsToOrder for Order ID {OrderId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddItemsToOrder ID {OrderId}.", id);
                return BadRequest(ModelState);
            }

            var updatedOrder = await _orderService.AddItemsToOrderAsync(id, orderItems);

            if (updatedOrder == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for adding items by API.", id);
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}/remove-items")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItemsFromOrder(int id, [FromBody] List<int> productIds)
        {
            _logger.LogInformation("API Endpoint called: RemoveItemsFromOrder for Order ID {OrderId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for RemoveItemsFromOrder ID {OrderId}.", id);
                return BadRequest(ModelState);
            }

            var updatedOrder = await _orderService.RemoveItemsFromOrderAsync(id, productIds);

            if (updatedOrder == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for removing items by API.", id);
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(updatedOrder);
        }
    }
}