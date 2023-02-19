using System.ComponentModel.DataAnnotations;

namespace PremiumCalculator.Models;

public class PremiumRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Range(1, 120)]
    public int Age { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string Occupation { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int DeathCoverAmount { get; set; }
}