using System.ComponentModel.DataAnnotations;

namespace AuthWith2Fa.Dtos.Request
{
    public class ResetPasswordDto
    {
        [Required]
        public string Password { get; set; } = string.Empty;

        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
