using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PremiumCalculator.Models;
using PremiumCalculator.Repository;
using PremiumCalculatorAPI.Controllers;
using Xunit;

namespace PremiumCalculatorTests
{
    public class PremiumCalculatorControllerTests
    {
        private readonly Mock<ILogger<PremiumCalculatorController>> _loggerMock;
        private readonly PremiumDbContext _dbContext;


        public PremiumCalculatorControllerTests()
        {
            _loggerMock = new Mock<ILogger<PremiumCalculatorController>>();
            _dbContext = new PremiumDbContext(CreateNewContextOptions());
        }

        [Fact]
        public async Task CalculatePremium_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new PremiumRequest
            {
                Name = "Raju",
                Age = 30,
                Occupation = "Doctor",
                DeathCoverAmount = 500000
            };
            var ratingFactor = 1.25;
            await GenerateInMemoryDB();
            var controller = new PremiumCalculatorController(_loggerMock.Object, _dbContext);
            
            // Act
            var result = await controller.CalculatePremium(request);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsType<double>(okResult.Value);
        }

        [Fact]
        public async Task CalculatePremium_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var controller = new PremiumCalculatorController(_loggerMock.Object, _dbContext);
            var request = new PremiumRequest
            {
                Name = "Somu",
                Age = -10,
                Occupation = "",
                DeathCoverAmount = -10000
            };

            // Act
            var result = await controller.CalculatePremium(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.IsType<List<ValidationResult>>(badRequestResult.Value);
            var validationResults = (List<ValidationResult>)badRequestResult.Value;
            Assert.Equal(3, validationResults.Count);
        }

        [Fact]
        public async Task CalculatePremium_InvalidOccupation_ReturnsBadRequestResult()
        {
            // Arrange
            var controller = new PremiumCalculatorController(_loggerMock.Object, _dbContext);
            var request = new PremiumRequest
            {
                Name = "Sam",
                Age = 30,
                Occupation = "Invalid Occupation",
                DeathCoverAmount = 500000
            };
      
            // Act
            var result = await controller.CalculatePremium(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.IsType<string>(badRequestResult.Value);
            var errorMessage = (string)badRequestResult.Value;
            Assert.Equal($"Invalid occupation: {request.Occupation}", errorMessage);
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
}