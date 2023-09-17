using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared.DTOS
{
    public class UserResetPassword
    {
        [Required, StringLength(100, MinimumLength = 6)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords Do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
