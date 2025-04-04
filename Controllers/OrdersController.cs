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
    public async Task<IActionResult> GetProducts()
    {
        var products = await _orderService.GetOrdersAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _orderService.GetOrderByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
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

        var updated = await _orderService.AddProductsToOrder(id, products);

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

        var updated = await _orderService.DeleteProductsFromOrder(id, productIds);
        
        if (updated == null)
        {
            return BadRequest("Failed to delete products from order.");
        }

        return Ok(updated);
    }
}
