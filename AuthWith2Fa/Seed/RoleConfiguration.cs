using AuthWith2Fa.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthWith2Fa.Seed
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(

                new Role
                {
                    Id = "7ad58efe-6c35-4d4a-acd8-60f6c1b75bcb",
                    Name = "Visitor",
                    Description = "visitor role"
                },
                 new Role
                 {
                     Id = "7ad58e2e-6c35-4d4a-acd8-60f6c1b7vbcb",
                     Name = "Admin",
                     Description = "visitor role"
                 });
        }
    }
}
