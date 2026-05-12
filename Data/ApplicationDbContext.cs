using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShadiHall.Models;

namespace ShadiHall.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Hall> Halls { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<BannerSlide> BannerSlides { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // Hall
            builder.Entity<Hall>(e =>
            {
                e.HasIndex(h => h.Name);
                e.Property(h => h.PricePerDay).HasColumnType("decimal(18,2)");
            });

            // Reservation
            builder.Entity<Reservation>(e =>
            {
                e.HasOne(r => r.User).WithMany(u => u.Reservations).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Hall).WithMany(h => h.Reservations).HasForeignKey(r => r.HallId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(r => r.ReservationCode).IsUnique();
            });

            // Review
            builder.Entity<Review>(e =>
            {
                e.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Hall).WithMany(h => h.Reviews).HasForeignKey(r => r.HallId).OnDelete(DeleteBehavior.Cascade);
            });

            // Payment
            builder.Entity<Payment>(e =>
            {
                e.HasOne(p => p.Reservation).WithOne(r => r.Payment).HasForeignKey<Payment>(p => p.ReservationId).OnDelete(DeleteBehavior.Cascade);
            });

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER", ConcurrencyStamp = "2" }
            );

            // Seed Banner Slides
            builder.Entity<BannerSlide>().HasData(
                new BannerSlide { SlideId = 1, Title = "Your Dream Wedding Awaits", Subtitle = "Elegant halls for your most special moments", ImageUrl = "/images/banner1.svg", CtaText = "Explore Halls", CtaLink = "/Hall", SortOrder = 1 },
                new BannerSlide { SlideId = 2, Title = "Celebrate in Style", Subtitle = "Premium venues with world-class amenities", ImageUrl = "/images/banner2.svg", CtaText = "Book Now", CtaLink = "/Reservation/Create", SortOrder = 2 },
                new BannerSlide { SlideId = 3, Title = "Unforgettable Memories", Subtitle = "Creating perfect moments since 2010", ImageUrl = "/images/banner3.svg", CtaText = "View Packages", CtaLink = "/Hall", SortOrder = 3 }
            );

            // Seed Admin User
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "admin-001",
                UserName = "admin@shadihall.com",
                NormalizedUserName = "ADMIN@SHADIHALL.COM",
                Email = "admin@shadihall.com",
                NormalizedEmail = "ADMIN@SHADIHALL.COM",
                EmailConfirmed = true,
                FullName = "System Administrator",
                Address = "Shadi Hall, Main Road, Chittagong",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123!");
            builder.Entity<ApplicationUser>().HasData(adminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" });

            // Seed Sample Halls
            builder.Entity<Hall>().HasData(
                new Hall { HallId = 1, Name = "Royal Grand Ballroom", Description = "Our flagship venue featuring a stunning 5,000 sq ft ballroom with soaring ceilings, crystal chandeliers, and state-of-the-art lighting systems. Perfect for grand weddings and galas.", ShortDescription = "5,000 sq ft luxury ballroom with crystal chandeliers", PricePerDay = 150000, Capacity = 800, Location = "Main Building, Ground Floor", ImageUrl = "/images/hall1.svg", IsAvailable = true, IsFeatured = true, Amenities = "AC,Sound System,Catering Kitchen,Bridal Suite,Parking,Generator" },
                new Hall { HallId = 2, Name = "Garden Terrace", Description = "A breathtaking outdoor garden venue surrounded by lush greenery and blooming flowers. Perfect for daytime ceremonies and intimate gatherings.", ShortDescription = "Outdoor garden venue with natural beauty", PricePerDay = 80000, Capacity = 400, Location = "East Wing, Outdoor", ImageUrl = "/images/hall2.svg", IsAvailable = true, IsFeatured = true, Amenities = "Open Air,Floral Decor,Catering,Photography Spots,Parking" },
                new Hall { HallId = 3, Name = "Diamond Conference Hall", Description = "A premium indoor venue with elegant decor, perfect for wedding receptions, corporate events, and social gatherings of all sizes.", ShortDescription = "Premium indoor venue with elegant modern decor", PricePerDay = 100000, Capacity = 600, Location = "West Wing, First Floor", ImageUrl = "/images/hall3.svg", IsAvailable = true, IsFeatured = false, Amenities = "AC,Projector,Stage,Sound System,Catering,Generator,Parking" },
                new Hall { HallId = 4, Name = "Jasmine Banquet", Description = "An intimate banquet hall with warm ambiance, ideal for smaller family gatherings, mehndi nights, and engagement ceremonies.", ShortDescription = "Intimate banquet hall for smaller gatherings", PricePerDay = 50000, Capacity = 200, Location = "Main Building, First Floor", ImageUrl = "/images/hall4.svg", IsAvailable = true, IsFeatured = false, Amenities = "AC,Sound System,Catering,Stage" }
            );
        }
    }
}
