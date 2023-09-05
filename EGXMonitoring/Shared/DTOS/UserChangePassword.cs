using System.ComponentModel.DataAnnotations;

namespace EGXMonitoring.Shared.DTOS
{
    public class UserChangePassword
    {
        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords Do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}