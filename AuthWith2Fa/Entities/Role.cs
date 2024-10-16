using Microsoft.AspNetCore.Identity;

namespace AuthWith2Fa.Entities
{
    public class Role : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
    }
}
