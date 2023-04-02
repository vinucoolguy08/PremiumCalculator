using MediatR;
using Microsoft.EntityFrameworkCore;
using PremiumCalculator.Controllers;
using PremiumCalculator.Models;
using PremiumCalculator.Repository;

namespace PremiumCalculator.Application;

public class PremiumRequestHandler : IRequestHandler<PremiumRequest, PremiumResponse>
{
    private readonly PremiumDbContext _dbContext;
    private readonly ILogger<PremiumCalculatorController> _logger;


    public PremiumRequestHandler(PremiumDbContext dbContext, ILogger<PremiumCalculatorController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _dbContext.Database.EnsureCreated();
    }

    public async Task<PremiumResponse> Handle(PremiumRequest request, CancellationToken cancellationToken)
    {
        // Get the occupation rating factor from the database
        var ratingFactor = await _dbContext.Occupations
            .Where(r => r.Name == request.Occupation)
            .Select(r => r.Rating.Factor)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        // Calculate the premium
        var deathPremium = (request.SumInsured * ratingFactor * request.Age) / 1000 * 12;
        _logger.LogInformation($"Calculated Premium for the {request.Name} is {Math.Round(deathPremium, 2)}");
        
        // Calculate TPD Premium
        var tpdPremium = (request.SumInsured * ratingFactor * request.Age) / 1234;
        _logger.LogInformation($"Calculated TPD Premium for the {request.Name} is {Math.Round(tpdPremium, 2)}");

        return new PremiumResponse
        {
            Name = request.Name,
            DeathPremium = Math.Round(deathPremium, 2),
            TpdPremium = Math.Round(tpdPremium, 2)
        };
    }
}