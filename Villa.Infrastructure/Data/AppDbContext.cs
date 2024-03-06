using Microsoft.EntityFrameworkCore;
using Villa.Domain.Entities;

namespace Villa.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Villa> Villas { get; set; }
    public DbSet<VillaNumber> VillaNumbers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Villa>().HasData(
            new Domain.Entities.Villa
            {
                Id = 1,
                Name = "Royal Villa",
                Description =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x400",
                Occupancy = 4,
                Price = 200,
                Sqm = 550,
            },
            new Domain.Entities.Villa
            {
                Id = 2,
                Name = "Premium Pool Villa",
                Description =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x401",
                Occupancy = 4,
                Price = 300,
                Sqm = 550,
            },
            new Domain.Entities.Villa
            {
                Id = 3,
                Name = "Luxury Pool Villa",
                Description =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x402",
                Occupancy = 4,
                Price = 400,
                Sqm = 750,
            });
        modelBuilder.Entity<VillaNumber>().HasData(
            new VillaNumber
            {
                Villa_Number = 101,
                VillaId = 1,
            },
            new VillaNumber
            {
                Villa_Number = 102,
                VillaId = 1,
            },
            new VillaNumber
            {
                Villa_Number = 103,
                VillaId = 1,
            },
            new VillaNumber
            {
                Villa_Number = 104,
                VillaId = 1,
            },
            new VillaNumber
            {
                Villa_Number = 201,
                VillaId = 2,
            },
            new VillaNumber
            {
                Villa_Number = 202,
                VillaId = 2,
            },
            new VillaNumber
            {
                Villa_Number = 203,
                VillaId = 2,
            },
            new VillaNumber
            {
                Villa_Number = 301,
                VillaId = 3,
            },
            new VillaNumber
            {
                Villa_Number = 302,
                VillaId = 3,
            }
        );
    }
}