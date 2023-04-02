using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PremiumCalculator.Controllers;
using PremiumCalculator.Models;
using Xunit;

namespace PremiumCalculatorTests
{
    public class PremiumCalculatorControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PremiumCalculatorController _controller;
        private readonly Fixture _fixture;

        public PremiumCalculatorControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PremiumCalculatorController(_mockMediator.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CalculatePremium_ReturnsOkObjectResult_WhenRequestIsValid()
        {
            // Arrange
            var request = _fixture.Create<PremiumRequest>();
            double expectedResult = 100;
            _mockMediator.Setup(x => x.Send(It.IsAny<PremiumRequest>(), CancellationToken.None))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CalculatePremium(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result!;
            Assert.Equal(expectedResult, okResult.Value);
        }
    }
}