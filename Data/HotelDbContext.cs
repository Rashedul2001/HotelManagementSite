using HotelManagementSite.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSite.Data
{
    public class HotelDbContext(DbContextOptions<HotelDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<ImageAndIcon> ImagesAndIcons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasMany(a => a.Accounts).WithOne(u => u.User).HasForeignKey(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany(b => b.Bookings).WithOne(u => u.User).HasForeignKey(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany(r => r.Reviews).WithOne(u => u.User).HasForeignKey(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasOne(v => v.VerificationToken).WithOne(u => u.User).HasForeignKey<VerificationToken>(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HotelRoom>().HasMany(b => b.Bookings).WithOne(h => h.HotelRoom).HasForeignKey(ui => ui.HotelRoomId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HotelRoom>().HasMany(hi => hi.HotelRoomImages).WithOne(h => h.HotelRoom).HasForeignKey(hri => hri.HotelRoomId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HotelRoom>().HasMany(r => r.Reviews).WithOne(h => h.HotelRoom).HasForeignKey(hri => hri.HotelRoomId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HotelRoom>().HasMany(a => a.Amenities).WithMany(h => h.HotelRooms).UsingEntity(j => j.ToTable("HotelRoomAmenities"));

        }

    }
}