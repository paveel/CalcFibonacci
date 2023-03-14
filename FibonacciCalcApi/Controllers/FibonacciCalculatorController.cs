using FibonacciCalc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FibonacciCalcApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class FibonacciCalculatorController : ControllerBase
    {
        private readonly FibonacciCalcService _fibonacciCalcService;
        public FibonacciCalculatorController(FibonacciCalcService fibonacciCalcService) 
        {
            _fibonacciCalcService = fibonacciCalcService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalcStep(int calcId, long value, CancellationToken cancellationToken = default)
        {
            try
            {
                await _fibonacciCalcService.CalcStepAsync(new FibonacciValue { CalcId = calcId, Value = value}, cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}
