using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineTechShop_WebApp.Models;
using OnlineTechShop_WebApp.Services;

namespace OnlineTechShop_WebApp.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Публічний доступ
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetActiveProductsAsync();
            return Ok(products);
        }

        // Admin: отримати будь-який товар
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }

        // Admin: створення
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Admin: оновлення
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            await _productService.UpdateAsync(id, product);
            return NoContent();
        }

        // Admin: деактивація
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _productService.DeactivateAsync(id);
            return NoContent();
        }
    }
}
