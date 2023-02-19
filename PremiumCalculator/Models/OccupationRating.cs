using System.ComponentModel.DataAnnotations;

namespace PremiumCalculator.Models;

public class OccupationRating
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public double Factor { get; set; }
}