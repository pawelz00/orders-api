using Microsoft.AspNetCore.Mvc;
using OrdersApi.Application.DTO.Product;
using OrdersApi.Application.Interfaces.Services; 

namespace OrdersApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("API Endpoint called: GetProducts");
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation("API Endpoint called: GetProduct with ID {ProductId}", id);
            
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found by API.", id);
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostProduct([FromBody] ProductCreate productDto)
        {
            _logger.LogInformation("API Endpoint called: PostProduct with Name {ProductName}", productDto.Name);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for PostProduct.");
                return BadRequest(ModelState);
            }

            var createdProduct = await _productService.CreateProductAsync(productDto);
            
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            _logger.LogInformation("API Endpoint called: PutProduct for ID {ProductId}", id);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for PutProduct ID {ProductId}.", id);
                return BadRequest(ModelState);
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, productDto);

            if (updatedProduct == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update by API.", id);
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            return Ok(updatedProduct); 
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("API Endpoint called: DeleteProduct for ID {ProductId}", id);
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for delete by API.", id);
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex) 
            {
                _logger.LogWarning(ex, "Cannot delete product ID {ProductId} due to dependencies.", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}