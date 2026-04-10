using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineTechShop_WebApp.Models;
using OnlineTechShop_WebApp.Services;
using System.Security.Claims;

namespace OnlineTechShop_WebApp.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Customer: створення
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(Dictionary<int, int> products)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderService.CreateOrderAsync(userId, products);
            return Ok(order);
        }

        // Customer: свої замовлення
        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(await _orderService.GetUserOrdersAsync(userId));
        }

        // Admin: всі замовлення
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            return Ok(await _orderService.GetAllOrdersAsync());
        }

        // Admin: зміна статусу
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
        {
            await _orderService.ChangeStatusAsync(id, status);
            return NoContent();
        }
    }
}
