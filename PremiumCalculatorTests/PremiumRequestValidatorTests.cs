using PremiumCalculator.Models;
using PremiumCalculator.Validator;
using Xunit;

namespace PremiumCalculatorTests;

public class PremiumRequestValidatorTests
{
    private readonly PremiumRequestValidator _validator;

    public PremiumRequestValidatorTests()
    {
        _validator = new PremiumRequestValidator();
    }

    [Fact]
    public void Given_ValidPremiumRequest_When_Validating_Then_ValidationSucceeds()
    {
        // Arrange
        var premiumRequest = new PremiumRequest
        {
            Name = "John Doe",
            Occupation = "Software Developer",
            Age = 35,
            DateOfBirth = new DateTime(1986, 1, 1),
            SumInsured = 500000
        };

        // Act
        var validationResult = _validator.Validate(premiumRequest);

        // Assert
        Assert.True(validationResult.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Given_InvalidName_When_Validating_Then_ValidationError(string name)
    {
        // Arrange
        var premiumRequest = new PremiumRequest
        {
            Name = name,
            Occupation = "Software Developer",
            Age = 35,
            DateOfBirth = new DateTime(1986, 1, 1),
            SumInsured = 500000
        };

        // Act
        var validationResult = _validator.Validate(premiumRequest);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains($"{nameof(PremiumRequest.Name)} should have value",
            validationResult.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public void Given_AgeLessThanTwo_When_Validating_Then_ValidationError()
    {
        // Arrange
        var premiumRequest = new PremiumRequest
        {
            Name = "John Doe",
            Occupation = "Software Developer",
            Age = 1,
            DateOfBirth = new DateTime(2022, 1, 1),
            SumInsured = 500000
        };

        // Act
        var validationResult = _validator.Validate(premiumRequest);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains($"{nameof(PremiumRequest.Age)} must be greater than 1.",
            validationResult.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public void Given_AgeGreaterThanSeventy_When_Validating_Then_ValidationError()
    {
        // Arrange
        var premiumRequest = new PremiumRequest
        {
            Name = "John Doe",
            Occupation = "Software Developer",
            Age = 80,
            DateOfBirth = new DateTime(1943, 1, 1),
            SumInsured = 500000
        };

        // Act
        var validationResult = _validator.Validate(premiumRequest);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains($"{nameof(PremiumRequest.Age)} must be less than 70.",
            validationResult.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public void Given_InvalidDateOfBirth_When_Validating_Then_ValidationError()
    {
        // Arrange
        var premiumRequest = new PremiumRequest
        {
            Name = "John Doe",
            Occupation = "Software Developer",
            Age = 35,
            DateOfBirth = default(DateTime),
            SumInsured = 500000
        };

        // Act
        var validationResult = _validator.Validate(premiumRequest);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains($"{nameof(PremiumRequest.DateOfBirth)} must be a valid date.",
            validationResult.Errors.Select(x => x.ErrorMessage));
    }
}