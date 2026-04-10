using Microsoft.EntityFrameworkCore;
using OnlineTechShop_WebApp.Models;

namespace OnlineTechShop_WebApp.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // Customer: створення замовлення
        public async Task<Order> CreateOrderAsync(int userId, Dictionary<int, int> products)
        {
            if (products == null || !products.Any())
                throw new InvalidOperationException("Order must contain at least one product");

            // Починаємо транзакцію: або все збережеться, або нічого
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var productIds = products.Keys.ToList();
                var dbProducts = await _context.Products
                    .Where(p => productIds.Contains(p.Id) && p.IsActive)
                    .ToListAsync();

                if (dbProducts.Count != products.Count)
                    throw new InvalidOperationException("One or more products are unavailable");

                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Created,
                    Items = new List<OrderItem>()
                };

                decimal total = 0;

                foreach (var product in dbProducts)
                {
                    var quantity = products[product.Id];

                    // захист від "читерства" з від'ємною кількістю
                    if (quantity <= 0)
                        throw new InvalidOperationException($"Quantity for product {product.Name} must be greater than zero.");

                    if (product.StockQuantity < quantity)
                        throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                    // 1. Зменшуємо залишок
                    product.StockQuantity -= quantity;

                    // 2. Створюємо позицію замовлення
                    var item = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        PriceAtPurchase = product.Price // Ціна фіксується!
                    };

                    total += item.PriceAtPurchase * quantity;
                    order.Items.Add(item);
                }

                order.TotalPrice = total;

                // 3. Зберігаємо все разом
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Підтверджуємо транзакцію
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                // Якщо щось пішло не так — відкочуємо зміни в базі
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Customer: свої замовлення
        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        // Admin: всі замовлення
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        // Admin: зміна статусу
        public async Task ChangeStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found");

            if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
                throw new InvalidOperationException("Final order status cannot be changed");

            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }
    }
}
