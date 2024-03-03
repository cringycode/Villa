using Microsoft.EntityFrameworkCore;

namespace Villa.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Villa> Villas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Villa>().HasData(
              new Domain.Entities.Villa
              {
                  Id = 1,
                  Name = "Royal Villa",
                  Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                  ImageUrl = "https://placehold.co/600x400",
                  Occupancy = 4,
                  Price = 200,
                  Sqm = 550,
              },
new Domain.Entities.Villa
{
    Id = 2,
    Name = "Premium Pool Villa",
    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
    ImageUrl = "https://placehold.co/600x401",
    Occupancy = 4,
    Price = 300,
    Sqm = 550,
},
new Domain.Entities.Villa
{
    Id = 3,
    Name = "Luxury Pool Villa",
    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
    ImageUrl = "https://placehold.co/600x402",
    Occupancy = 4,
    Price = 400,
    Sqm = 750,
}
            );
    }
}