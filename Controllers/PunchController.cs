using Microsoft.AspNetCore.Mvc;
using TimeCalculator.Interfaces;
using TimeCalculator.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PunchController : ControllerBase
    {
        private readonly IPunchService _punchService;
        public PunchController(IPunchService punchService)
        {
            _punchService = punchService;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromQuery] string punchData)
        {
            if (punchData == null || !punchData.Any())
            {
                return BadRequest("No punch times provided.");
            }
          
            var punchTimes  = _punchService.CreatePunchData(punchData);

            TimeSpan totalWorked = TimeSpan.Zero;
            foreach (var punchTime in punchTimes)
            {
                if (punchTime.PunchOut.HasValue)
                {
                    totalWorked += punchTime.PunchOut.Value - punchTime.PunchIn;
                }
                else
                {
                   
                    totalWorked += DateTime.Now - punchTime.PunchIn;
                }
            }

           
            TimeSpan targetWorkTime = TimeSpan.FromHours(8);
            TimeSpan remainingTime = targetWorkTime - totalWorked;

            
            DateTime completionTime = DateTime.Now.Add(remainingTime);

            
            var result = new
            {
                error = false,
                status = true,
                message = "Punchin time",
                data = new
                {
                    TotalTimeWorked = totalWorked,
                    RemainingTime = remainingTime,
                    CompletionTime = completionTime.ToString("hh:mm:ss tt")
                }
            };

            return Ok(result);
        }
    }
}
