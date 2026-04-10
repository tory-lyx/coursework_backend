using System.ComponentModel.DataAnnotations;

namespace OnlineTechShop_WebApp.Models
{
    public class AuthorisationModel
    {
        [Required(ErrorMessage = "Login is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
