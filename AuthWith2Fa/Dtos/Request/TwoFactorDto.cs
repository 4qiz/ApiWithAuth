using System.ComponentModel.DataAnnotations;

namespace AuthWith2Fa.Dtos.Request
{
    public class TwoFactorDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Provider { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
