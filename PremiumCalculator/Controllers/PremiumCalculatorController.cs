using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PremiumCalculator.Models;
using PremiumCalculator.Repository;

namespace PremiumCalculatorAPI.Controllers
{
    [ApiController]
    [Route("PremiumCalculator")]
    public class PremiumCalculatorController : ControllerBase
    {
        private readonly ILogger<PremiumCalculatorController> _logger;
        private readonly PremiumDbContext _dbContext;

        public PremiumCalculatorController(ILogger<PremiumCalculatorController> logger, PremiumDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        [HttpPost]
        public async Task<ActionResult<decimal>> CalculatePremium([FromBody] PremiumRequest request)
        {
            // Validate the input model
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
            {
                _logger.LogError("Request validation failed: {0}", string.Join(", ", validationResults));
                return BadRequest(validationResults);
            }
            
            // Get the occupation rating factor from the database
            var ratingFactor = await _dbContext.Occupations
                .Where(r => r.Name == request.Occupation)
                .Select(r => r.Rating.Factor)
                .FirstOrDefaultAsync();

            if (ratingFactor == 0)
            {
                _logger.LogError("Invalid occupation: {Occupation}", request.Occupation);
                return BadRequest($"Invalid occupation: {request.Occupation}");
            }

            // Calculate the premium
            var deathPremium = (request.DeathCoverAmount * ratingFactor * request.Age) / 1000 * 12;

            return Ok(Math.Round(deathPremium, 2));
        }
    }
}

   


    