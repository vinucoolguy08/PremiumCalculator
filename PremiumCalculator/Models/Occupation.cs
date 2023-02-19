using System.ComponentModel.DataAnnotations;

namespace PremiumCalculator.Models;

public class Occupation
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int RatingId { get; set; }
    
    public OccupationRating Rating { get; set; }
    
}