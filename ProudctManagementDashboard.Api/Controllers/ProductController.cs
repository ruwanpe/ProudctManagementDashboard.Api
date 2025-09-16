using Microsoft.AspNetCore.Mvc;
using ProudctManagementDashboard.Api.Cache;
using ProudctManagementDashboard.Api.Helper;
using ProudctManagementDashboard.Api.Models;
using ProudctManagementDashboard.Api.Repository;

namespace ProudctManagementDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly IMemeoryCacheService _memoryCache;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo productRepo, IMemeoryCacheService memoryCache, ILogger<ProductController> logger)
        {
            _productRepo = productRepo;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> AddProudct(Product product)
        {
            try
            {
                _logger.LogInformation("Adding a new product.");
                var response = await _productRepo.RegisterProduct(product);
                return response > 0 ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new product.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("Fetching all products from cache or database.");
                List<Product> result = _memoryCache.Get<List<Product>>(CacheKeyEnum.AllProducts.ToString());

                if (result != null && result.Count > 0)
                {
                    return result;
                }

                result = await _productRepo.GetAllProducts();
                _memoryCache.Set<List<Product>>(CacheKeyEnum.AllProducts.ToString(), result, TimeSpan.FromMinutes(5));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetAllProductsByCategory")]
        public async Task<ActionResult<string>> GetAllProductsByCategory()
        {
            try
            {
                _logger.LogInformation("Fetching products by category from cache or database.");
                string result = _memoryCache.Get<string>(CacheKeyEnum.ProductsByCategory.ToString());
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }

                result = await _productRepo.GetAllProductsByCategory();
                _memoryCache.Set(CacheKeyEnum.ProductsByCategory.ToString(), result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products by category.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetAllProductsByDurationAdded")]
        public async Task<ActionResult<string>> GetAllProductsByDurationAdded()
        {
            try
            {
                _logger.LogInformation("Fetching products by duration added from cache or database.");
                string result = _memoryCache.Get<string>(CacheKeyEnum.ProductsByDurationAdded.ToString());
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }

                result = await _productRepo.GetAllProductsByDurationAdded();
                _memoryCache.Set(CacheKeyEnum.ProductsByDurationAdded.ToString(), result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products by duration added.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
