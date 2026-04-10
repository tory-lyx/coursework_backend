using Microsoft.EntityFrameworkCore;
using OnlineTechShop_WebApp.Models;

namespace OnlineTechShop_WebApp.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // Публічний каталог (тільки активні)
        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        // Отримати товар по Id (для admina)
        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return product;
        }

        // Admin: створення товару
        public async Task<Product> CreateAsync(Product product)
        {
            if (product.Price <= 0)
                throw new InvalidOperationException("Price must be greater than zero");

            if (product.StockQuantity < 0)
                throw new InvalidOperationException("Stock cannot be negative");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        // Admin: оновлення
        public async Task UpdateAsync(int id, Product updated)
        {
            var product = await GetByIdAsync(id);

            product.Name = updated.Name;
            product.Price = updated.Price;
            product.StockQuantity = updated.StockQuantity;
            product.IsActive = updated.IsActive;

            await _context.SaveChangesAsync();
        }

        // Admin: деактивація
        public async Task DeactivateAsync(int id)
        {
            var product = await GetByIdAsync(id);
            product.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
