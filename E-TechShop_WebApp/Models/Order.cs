using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineTechShop_WebApp.Models
{
    public enum OrderStatus
    {
        Created = 0,
        Paid = 1,
        Shipped = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }
}
