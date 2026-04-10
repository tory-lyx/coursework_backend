using System.ComponentModel.DataAnnotations;

namespace OnlineTechShop_WebApp.Models
{
    public enum UserRole
    {
        Customer = 0,
        Admin = 1
    }

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Login is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters")]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
