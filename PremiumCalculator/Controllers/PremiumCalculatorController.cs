using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremiumCalculator.Models;

namespace PremiumCalculator.Controllers
{
    [ApiController]
    [Route("PremiumCalculator")]
    public class PremiumCalculatorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PremiumCalculatorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<PremiumResponse>> CalculatePremium([FromBody] PremiumRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}

   


    