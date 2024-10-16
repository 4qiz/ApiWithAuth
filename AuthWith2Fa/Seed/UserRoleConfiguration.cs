using AuthWith2Fa.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthWith2Fa.Seed
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                RoleId = "7ad58e2e-6c35-4d4a-acd8-60f6c1b7vbcb",
                UserId = "7ad58e2e-6c35-4d4a-acd8-60f6c1b75bcb"
            });
        }
    }
}
