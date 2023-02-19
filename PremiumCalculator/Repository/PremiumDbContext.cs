using Microsoft.EntityFrameworkCore;
using PremiumCalculator.Models;

namespace PremiumCalculator.Repository;

public class PremiumDbContext : DbContext
{
    public PremiumDbContext(DbContextOptions<PremiumDbContext> options) : base(options)
    {
    }

    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<OccupationRating> OccupationRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Occupation>().HasData(
            new Occupation { Id = 1, Name = "Cleaner", RatingId = 3 },
            new Occupation { Id = 2, Name = "Doctor", RatingId = 1 },
            new Occupation { Id = 3, Name = "Author", RatingId = 2 },
            new Occupation { Id = 4, Name = "Farmer", RatingId = 4 },
            new Occupation { Id = 5, Name = "Mechanic", RatingId = 4 },
            new Occupation { Id = 6, Name = "Florist", RatingId = 3 }
        );

        modelBuilder.Entity<OccupationRating>().HasData(
            new OccupationRating { Id = 1, Name = "Professional", Factor = 1.0 },
            new OccupationRating { Id = 2, Name = "White Collar", Factor = 1.25 },
            new OccupationRating { Id = 3, Name = "Light Manual", Factor = 1.50 },
            new OccupationRating { Id = 4, Name = "Heavy Manual", Factor = 1.75 }
        );
    }
}




