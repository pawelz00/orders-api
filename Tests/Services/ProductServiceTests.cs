using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OrdersApi.Domain.Interfaces.Repositories;
using OrdersApi.Application.Services;
using OrdersApi.Domain.Models;

namespace OrdersApi.Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_productRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Produkt 1", Price = 1 };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId))
                                  .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal(expectedProduct.Name, result.Name);
            Assert.Equal(expectedProduct.Price, result.Price);
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 99;
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId))
                                  .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
        }
    }
}