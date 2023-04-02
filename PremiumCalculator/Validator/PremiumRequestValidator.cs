using PremiumCalculator.Models;
using FluentValidation;

namespace PremiumCalculator.Validator;

public class PremiumRequestValidator : AbstractValidator<PremiumRequest>
{
    public PremiumRequestValidator()
    {
        RuleFor(z => z.Name)
            .NotEmpty()
            .WithMessage($"{nameof(PremiumRequest.Name)} should have value");
        
        RuleFor(z => z.Occupation)
            .NotEmpty()
            .WithMessage($"{nameof(PremiumRequest.Occupation)} should have value");
        
        RuleFor(z => z.Age)
            .GreaterThan(1).WithMessage($"{nameof(PremiumRequest.Age)} must be greater than 1.")
            .LessThan(70).WithMessage($"{nameof(PremiumRequest.Age)} must be less than 70.");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage($"{nameof(PremiumRequest.DateOfBirth)} is required.")
            .Must(BeAValidDate).WithMessage($"{nameof(PremiumRequest.DateOfBirth)} must be a valid date.")
            .LessThan(DateTime.Now).WithMessage($"{nameof(PremiumRequest.DateOfBirth)} must be in the past.");
        
        RuleFor(x => x.SumInsured)
            .NotEmpty().WithMessage("Sum insured is required.")
            .GreaterThan(0).WithMessage($"{nameof(PremiumRequest.SumInsured)} must be greater than 0.")
            .LessThanOrEqualTo(int.MaxValue).WithMessage($"{nameof(PremiumRequest.SumInsured)} must be less than or equal to {int.MaxValue}.");
    }
    
    private bool BeAValidDate(DateTime date)
    {
        return !date.Equals(default(DateTime));
    }
}