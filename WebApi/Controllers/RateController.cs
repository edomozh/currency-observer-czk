using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly RateService _rateService;

        public RateController(RateService rateService)
        {
            _rateService = rateService;
        }

        [HttpGet]
        public IActionResult Get(string currencyCode, string ddMMyyyyDate)
        {
            try
            {
                var rate = _rateService.GetRate(currencyCode, ddMMyyyyDate);
                return Ok(rate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
