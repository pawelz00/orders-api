using Microsoft.AspNetCore.Mvc;
using orders_api.DTO.Order;
using orders_api.Services;

namespace orders_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var products = await _orderService.GetOrdersAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var product = await _orderService.GetOrderByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> PostOrder([FromBody] OrderCreate order)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdOrder = await _orderService.CreateOrderAsync(order);

        if (createdOrder == null)
        {
            return BadRequest("Failed to create order.");
        }

        return CreatedAtAction(nameof(PostOrder), createdOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrder(int id, [FromBody] OrderUpdate order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        
        if (existingOrder == null)
        {
            return NotFound();
        }

        var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
        
        if (updatedOrder == null)
        {
            return BadRequest("Failed to update order.");
        }

        return Ok(updatedOrder);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var deleted = await _orderService.DeleteOrderAsync(id);

        if (deleted == false)
        {
            return BadRequest("Failed to delete order.");
        }

        return NoContent();
    }

    [HttpPost("{id}/add-products")]
    public async Task<IActionResult> AddProductsToOrder(int id, [FromBody] List<OrderItemCreate> products)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var updated = await _orderService.AddProductsToOrderAsync(id, products);

        if (updated == null)
        {
            return BadRequest("Failed to add products to order.");
        }


        return Ok(updated);
    }

    [HttpPost("{id}/delete-products")]
    public async Task<IActionResult> DeleteProductsFromOrder(int id, [FromBody] List<int> productIds)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var updated = await _orderService.DeleteProductsFromOrderAsync(id, productIds);
        
        if (updated == null)
        {
            return BadRequest("Failed to delete products from order.");
        }

        return Ok(updated);
    }
}
