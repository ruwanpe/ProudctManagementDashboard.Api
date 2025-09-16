using ProudctManagementDashboard.Api.Models;

namespace ProudctManagementDashboard.Api.Repository
{
    public interface IProductRepo
    {
        Task<int> RegisterProduct(Product product);
        Task<List<Product>> GetAllProducts();
        Task<string> GetAllProductsByCategory();
        Task<string> GetAllProductsByDurationAdded();
    }
}
