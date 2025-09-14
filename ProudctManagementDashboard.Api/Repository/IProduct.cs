using ProudctManagementDashboard.Api.Models;

namespace ProudctManagementDashboard.Api.Repository
{
    public interface IProduct
    {
        Task<int> RegisterProduct(Product product);
        Task<List<Product>> GetAllProducts();
        Task<string> GetAllProductsByCategory();
        Task<string> GetAllProductsByDurationAdded();
    }
}
