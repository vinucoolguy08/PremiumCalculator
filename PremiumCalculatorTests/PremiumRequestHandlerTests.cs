using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PremiumCalculator.Application;
using PremiumCalculator.Controllers;
using PremiumCalculator.Models;
using PremiumCalculator.Repository;
using Xunit;

namespace PremiumCalculatorTests;

public class PremiumRequestHandlerTests
{
    private readonly PremiumDbContext _dbContext;
    private readonly Mock<ILogger<PremiumCalculatorController>> _mockLogger;
    private PremiumRequestHandler _handler;

    public PremiumRequestHandlerTests()
    {
        _dbContext = new PremiumDbContext(CreateNewContextOptions());
        _mockLogger = new Mock<ILogger<PremiumCalculatorController>>();
    }

    [Fact]
    public async Task Handle_ReturnsCorrectPremium_WhenRatingFactorExistsInDatabase()
    {
        // Arrange
        var request = new PremiumRequest
        {
            Name = "Raju",
            Age = 30,
            Occupation = "Doctor",
            SumInsured = 500
        };
        _handler = new PremiumRequestHandler(_dbContext, _mockLogger.Object);
        var expectedPremium = new PremiumResponse
        {
            Name = "Raju",
            DeathPremium = 180,
            TpdPremium = 12.16
        };
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPremium.DeathPremium, result.DeathPremium);
        Assert.Equal(expectedPremium.TpdPremium, result.TpdPremium);
    }
    
    [Fact]
    public async Task Handle_ReturnsZero_WhenRatingFactorDoesNotExistInDatabase()
    {
        // Arrange
        var request = new PremiumRequest
        {
            Name = "Raju",
            Age = 30,
            Occupation = "Nurse",
            SumInsured = 500
        };
        _handler = new PremiumRequestHandler(_dbContext, _mockLogger.Object);
    
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.Equal(0, result.DeathPremium);
        Assert.Equal(0, result.TpdPremium);

    }
    
    private async Task GenerateInMemoryDB()
    {
        await _dbContext.Occupations.AddRangeAsync(
            new Occupation { Id = 1, Name = "Cleaner", RatingId = 3 },
            new Occupation { Id = 2, Name = "Doctor", RatingId = 1 },
            new Occupation { Id = 3, Name = "Author", RatingId = 2 },
            new Occupation { Id = 4, Name = "Farmer", RatingId = 4 },
            new Occupation { Id = 5, Name = "Mechanic", RatingId = 4 },
            new Occupation { Id = 6, Name = "Florist", RatingId = 3 }
        );
        
        await _dbContext.OccupationRatings.AddRangeAsync(
            new OccupationRating { Id = 1, Name = "Professional", Factor = 1.0 },
            new OccupationRating { Id = 2, Name = "White Collar", Factor = 1.25 },
            new OccupationRating { Id = 3, Name = "Light Manual", Factor = 1.50 },
            new OccupationRating { Id = 4, Name = "Heavy Manual", Factor = 1.75 }
        );
    }
    
    private DbContextOptions<PremiumDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh 
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<PremiumDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

}