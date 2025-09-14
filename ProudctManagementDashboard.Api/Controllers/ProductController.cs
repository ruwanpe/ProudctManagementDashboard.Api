using Microsoft.AspNetCore.Http;
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
        private readonly IProduct _productInterface;
        private readonly IMemeoryCacheService _memoryCache;

        public ProductController(IProduct productInterface, IMemeoryCacheService memoryCache)
        {
            _productInterface = productInterface;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        public async Task<ActionResult> AddProudct(Product product)
        {
            try
            {
                var response = await _productInterface.RegisterProduct(product);
                return response > 0 ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                List<Product> result = _memoryCache.Get<List<Product>>(CacheKeyEnum.AllProducts.ToString());

                if (result != null && result.Count > 0)
                {
                    return result;
                }

                result = await _productInterface.GetAllProducts();
                _memoryCache.Set<List<Product>>(CacheKeyEnum.AllProducts.ToString(), result, TimeSpan.FromMinutes(5));
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetAllProductsByCategory")]
        public async Task<ActionResult<string>> GetAllProductsByCategory()
        {
            try
            {
                string cacheResult = _memoryCache.Get<string>(CacheKeyEnum.ProductsByCategory.ToString());
                if (!string.IsNullOrEmpty(cacheResult))
                {
                    return cacheResult;
                }

                string dbResult = await _productInterface.GetAllProductsByCategory();
                _memoryCache.Set(CacheKeyEnum.ProductsByCategory.ToString(), dbResult, TimeSpan.FromMinutes(5));

                return dbResult;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetAllProductsByDurationAdded")]
        public async Task<ActionResult<string>> GetAllProductsByDurationAdded()
        {
            try
            {
                string cacheResult = _memoryCache.Get<string>(CacheKeyEnum.ProductsByDurationAdded.ToString());
                if (!string.IsNullOrEmpty(cacheResult))
                {
                    return cacheResult;
                }

                string dbResult = await _productInterface.GetAllProductsByDurationAdded();
                _memoryCache.Set(CacheKeyEnum.ProductsByDurationAdded.ToString(), dbResult, TimeSpan.FromMinutes(5));

                return dbResult;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
