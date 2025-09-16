using Microsoft.EntityFrameworkCore;
using ProudctManagementDashboard.Api.Data;
using ProudctManagementDashboard.Api.Models;
using System.Text.Json;

namespace ProudctManagementDashboard.Api.Repository
{
    public class ProductRepo(ProductDbContext context) : IProductRepo
    {
        public async Task<int> RegisterProduct(Product product)
        {
            context.Products.Add(product);
            return await context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<string> GetAllProductsByCategory()
        {
            var dbResult = await context.Products
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(p => p.StockQuantity)
                }).ToArrayAsync();

            string jsonData = JsonSerializer.Serialize(dbResult);

            return jsonData;

        }

        public async Task<string> GetAllProductsByDurationAdded()
        {
            DateTime thisWeek = DateTime.Now.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Sunday);
            int productsAddedthisWeek = await context.Products
                .Where(p => p.DateAdded.Date >= thisWeek.Date)
                .SumAsync(p => p.StockQuantity);
            string thisWeekLabel = thisWeek.ToString("dd MMM yyyy") + " - " + DateTime.Today.ToString("dd MMM yyyy");

            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int productsAddedthisMonth = await context.Products
                .Where(p => p.DateAdded.Date >= thisMonth.Date)
                .SumAsync(p => p.StockQuantity);
            string thisMonthLabel = thisMonth.ToString("MMM yyyy");

            DateTime thisYear = new DateTime(DateTime.Now.Year, 1, 1);
            int productsAddedthisYear = await context.Products
                .Where(p => p.DateAdded.Date >= thisYear.Date)
                .SumAsync(p => p.StockQuantity);
            string thisYearLabel = "Year " + thisYear.ToString("yyyy");

            var result = new object[]
            {
                new { Duration = thisWeekLabel, ProductsAdded = productsAddedthisWeek },
                new { Duration = thisMonthLabel, ProductsAdded = productsAddedthisMonth },
                new { Duration = thisYearLabel, ProductsAdded = productsAddedthisYear }
            };

            string jsonData = JsonSerializer.Serialize(result);

            return jsonData;
        }
    }
}
