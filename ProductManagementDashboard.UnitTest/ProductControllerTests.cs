using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ProudctManagementDashboard.Api.Controllers;
using ProudctManagementDashboard.Api.Models;
using ProudctManagementDashboard.Api.Repository;
using ProudctManagementDashboard.Api.Cache;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ProductManagementDashboard.UnitTest
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductRepo> _productRepoMock;
        private readonly Mock<IMemeoryCacheService> _cacheMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _productRepoMock = new Mock<IProductRepo>();
            _cacheMock = new Mock<IMemeoryCacheService>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_productRepoMock.Object, _cacheMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddProudct_ReturnsOk_WhenProductAdded()
        {
            Product product = new Product { Id = 1, Name = "Test", Category = "Cat" };
            _productRepoMock.Setup(p => p.RegisterProduct(product)).ReturnsAsync(1);

            IActionResult result = await _controller.AddProudct(product);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, okResult.Value);
        }

        [Fact]
        public async Task AddProudct_ReturnsBadRequest_WhenProductNotAdded()
        {
            Product product = new Product { Id = 1, Name = "Test", Category = "Cat" };
            _productRepoMock.Setup(p => p.RegisterProduct(product)).ReturnsAsync(0);

            IActionResult result = await _controller.AddProudct(product);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddProudct_ReturnsServerError_OnException()
        {
            Product product = new Product { Id = 1, Name = "Test", Category = "Cat" };
            _productRepoMock.Setup(p => p.RegisterProduct(product)).ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.AddProudct(product);

            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsFromCache_IfExists()
        {
            List<Product> products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _cacheMock.Setup(c => c.Get<List<Product>>("AllProducts")).Returns(products);

            ActionResult<List<Product>> result = await _controller.GetAllProducts();

            var okResult = Assert.IsType<ActionResult<List<Product>>>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsFromDb_IfCacheEmpty()
        {
            _cacheMock.Setup(c => c.Get<List<Product>>("AllProducts")).Returns((List<Product>)null);
            List<Product> products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _productRepoMock.Setup(p => p.GetAllProducts()).ReturnsAsync(products);

            ActionResult<List<Product>> result = await _controller.GetAllProducts();

            var okResult = Assert.IsType<ActionResult<List<Product>>>(result);
            Assert.Equal(products, okResult.Value);
            _cacheMock.Verify(c => c.Set("AllProducts", products, It.IsAny<TimeSpan>(), null), Times.Once);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsServerError_OnException()
        {
            _cacheMock.Setup(c => c.Get<List<Product>>("AllProducts")).Throws(new Exception("fail"));

            ActionResult<List<Product>> result = await _controller.GetAllProducts();

            var serverError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsFromCache_IfExists()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByCategory")).Returns("cached");

            ActionResult<string> result = await _controller.GetAllProductsByCategory();

            Assert.Equal("cached", result.Value);
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsFromDb_IfCacheEmpty()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByCategory")).Returns((string)null);
            _productRepoMock.Setup(p => p.GetAllProductsByCategory()).ReturnsAsync("dbResult");

            ActionResult<string> result = await _controller.GetAllProductsByCategory();

            Assert.Equal("dbResult", result.Value);
            _cacheMock.Verify(c => c.Set("ProductsByCategory", "dbResult", It.IsAny<TimeSpan>(), null), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsServerError_OnException()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByCategory")).Throws(new Exception("fail"));

            ActionResult<string> result = await _controller.GetAllProductsByCategory();

            var serverError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task GetAllProductsByDurationAdded_ReturnsFromCache_IfExists()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByDurationAdded")).Returns("cached");

            ActionResult<string> result = await _controller.GetAllProductsByDurationAdded();

            Assert.Equal("cached", result.Value);
        }

        [Fact]
        public async Task GetAllProductsByDurationAdded_ReturnsFromDb_IfCacheEmpty()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByDurationAdded")).Returns((string)null);
            _productRepoMock.Setup(p => p.GetAllProductsByDurationAdded()).ReturnsAsync("dbResult");

            ActionResult<string> result = await _controller.GetAllProductsByDurationAdded();

            Assert.Equal("dbResult", result.Value);
            _cacheMock.Verify(c => c.Set("ProductsByDurationAdded", "dbResult", It.IsAny<TimeSpan>(), null), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsByDurationAdded_ReturnsServerError_OnException()
        {
            _cacheMock.Setup(c => c.Get<string>("ProductsByDurationAdded")).Throws(new Exception("fail"));

            ActionResult<string> result = await _controller.GetAllProductsByDurationAdded();

            var serverError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverError.StatusCode);
        }
    }
}
