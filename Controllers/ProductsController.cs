using Microsoft.AspNetCore.Mvc;
using orders_api.Models;
using orders_api.Services;
using orders_api.DTO.Product;

namespace orders_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var productResponse = await _productService.CreateProductAsync(product);

        return CreatedAtAction(nameof(PostProduct), productResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, [FromBody] ProductUpdateDto productDto)
    {
        // Todo: validation

        var updatedProduct = await _productService.UpdateProductAsync(id, productDto);

        if (updatedProduct == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var deleted = await _productService.DeleteProductAsync(id);

        if (deleted == false)
        {
            return BadRequest("Product is ordered - remove order first!");
        }

        return NoContent();
    }
}
