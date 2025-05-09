using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSite.Data
{
    public class HotelAuthDbContext(DbContextOptions<HotelAuthDbContext> options) : IdentityDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "c7c52db5-13c6-42d7-bb49-8aece5c3fd02",
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = "f2b0a1d4-3c5e-4f8b-9a7c-6d5e0f3b2c8f"
                },
                new IdentityRole
                {
                    Id = "10716f79-75e2-4f45-905f-dbe4f97e67fb",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "a00b55e6-e84e-4a07-8529-de3a0474c3b0"
                },
                new IdentityRole
                {
                    Id = "78318522-43a2-459e-9b1f-9851eb390610",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "451b4ac8-087f-40d2-a223-674c54a35ffc"
                }
            );
        }
    }
}