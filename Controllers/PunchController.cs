using Microsoft.AspNetCore.Mvc;
using TimeCalculator.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PunchController : ControllerBase
    {
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] List<PunchModel> punchTimes)
        {
            if (punchTimes == null || !punchTimes.Any())
            {
                return BadRequest("No punch times provided.");
            }
          
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
                TotalTimeWorked = totalWorked,
                RemainingTime = remainingTime,
                CompletionTime = completionTime.ToString("hh:mm:ss tt")
            };

            return Ok(result);
        }
    }
}
