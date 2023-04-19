using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }

        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}