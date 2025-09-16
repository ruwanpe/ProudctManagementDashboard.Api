using Microsoft.EntityFrameworkCore;
using ProudctManagementDashboard.Api.Models;
using ProudctManagementDashboard.Api.Repository;
using ProudctManagementDashboard.Api.Data;
using System.Text.Json;

namespace ProductManagementDashboard.UnitTest
{   
    public class ProductRepoTests
    {
        private ProductDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ProductDbContext(options);
        }

        [Fact]
        public async Task RegisterProduct_AddsProduct_ReturnsOne()
        {
            ProductDbContext context = GetDbContext(nameof(RegisterProduct_AddsProduct_ReturnsOne));
            ProductRepo repo = new ProductRepo(context);
            Product product = new Product
            {
                Name = "Test",
                ProductCode = "Code 1",
                Category = "Cat",
                Price = 10,
                SKU = "SKU1",
                StockQuantity = 5,
                DateAdded = DateTime.Now
            };

            var result = await repo.RegisterProduct(product);

            Assert.Equal(1, result);
            Assert.Single(context.Products);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            ProductDbContext context = GetDbContext(nameof(GetAllProducts_ReturnsAllProducts));
            context.Products.AddRange(
                new Product { Name = "A", ProductCode= "Code 1", Category = "C1", Price = 1, SKU = "S1", StockQuantity = 2, DateAdded = DateTime.Now },
                new Product { Name = "B", ProductCode= "Code 2", Category = "C2", Price = 2, SKU = "S2", StockQuantity = 3, DateAdded = DateTime.Now }
            );
            context.SaveChanges();

            ProductRepo repo = new ProductRepo(context);

            List<Product> result = await repo.GetAllProducts();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "A");
            Assert.Contains(result, p => p.Name == "B");
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsCorrectJson()
        {
            ProductDbContext context = GetDbContext(nameof(GetAllProductsByCategory_ReturnsCorrectJson));
            context.Products.AddRange(
                new Product { Name = "A", ProductCode = "Code 1", Category = "C1", Price = 1, SKU = "S1", StockQuantity = 2, DateAdded = DateTime.Now },
                new Product { Name = "B", ProductCode = "Code 2", Category = "C1", Price = 2, SKU = "S2", StockQuantity = 3, DateAdded = DateTime.Now },
                new Product { Name = "C", ProductCode = "Code 3", Category = "C2", Price = 3, SKU = "S3", StockQuantity = 4, DateAdded = DateTime.Now }
            );
            context.SaveChanges();
            ProductRepo repo = new ProductRepo(context);

            string json = await repo.GetAllProductsByCategory();
            List<CategoryResult> result = JsonSerializer.Deserialize<List<CategoryResult>>(json) ?? [];

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.CategoryName == "C1" && r.TotalQuantity == 5);
            Assert.Contains(result, r => r.CategoryName == "C2" && r.TotalQuantity == 4);
        }

        [Fact]
        public async Task GetAllProductsByDurationAdded_ReturnsCorrectJson()
        {
            DateTime now = DateTime.Now;
            DateTime thisMonth = new DateTime(now.Year, now.Month, 1);
            DateTime thisYear = new DateTime(now.Year, 1, 1);
            ProductDbContext context = GetDbContext(nameof(GetAllProductsByDurationAdded_ReturnsCorrectJson));
            context.Products.AddRange(
                new Product { Name = "A", ProductCode = "Code 1", Category = "C1", Price = 1, SKU = "S1", StockQuantity = 2, DateAdded = now },
                new Product { Name = "B", ProductCode = "Code 2", Category = "C2", Price = 2, SKU = "S2", StockQuantity = 3, DateAdded = thisMonth },
                new Product { Name = "C", ProductCode = "Code 3", Category = "C2", Price = 3, SKU = "S3", StockQuantity = 4, DateAdded = thisYear }
            );
            context.SaveChanges();
            ProductRepo repo = new ProductRepo(context);

            string json = await repo.GetAllProductsByDurationAdded();
            List<DurationResult> result = JsonSerializer.Deserialize<List<DurationResult>>(json) ?? [];

            Assert.Equal(3, result.Count);
            Assert.All(result, r => Assert.True(r.ProductsAdded >= 0));
        }

        private class CategoryResult
        {
            public string CategoryName { get; set; }
            public int TotalQuantity { get; set; }
        }

        private class DurationResult
        {
            public string Duration { get; set; }
            public int ProductsAdded { get; set; }
        }
    }
}
