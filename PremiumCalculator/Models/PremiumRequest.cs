using MediatR;

namespace PremiumCalculator.Models;

public class PremiumRequest : IRequest<PremiumResponse>
{
    public string Name { get; set; }

    public int Age { get; set; }
    
    public DateTime DateOfBirth { get; set; }

    public string Occupation { get; set; }
    
    public int SumInsured { get; set; }
}