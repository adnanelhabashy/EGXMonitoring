using System.ComponentModel.DataAnnotations;

namespace EGXMonitoring.Shared.DTOS
{
    public class UserRegister
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}