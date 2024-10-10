using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleJwtAuth.Models;

namespace SimpleJwtAuth.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Blogs> Blogs{ get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    }
}
