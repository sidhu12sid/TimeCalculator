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

            var lastPunchOut = punchTimes[punchTimes.Count - 1].PunchOut;

            string? output = null;

            TimeSpan targetWorkTime = TimeSpan.FromHours(8);

            if(totalWorked >= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = totalWorked - targetWorkTime;              
                var completedTime = Convert.ToDateTime(lastPunchOut) - workDifference;
                output = $"You have completed 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")} and you have {Math.Round(workDifference.TotalMinutes, 2)} minutes as extra time";
            }
            else if(totalWorked <= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = targetWorkTime - totalWorked;
                var completedTime = Convert.ToDateTime(lastPunchOut) + workDifference;
                output = $"You are {Math.Round(workDifference.TotalMinutes, 2)} minutes deficit for attaining 8 hours You could have been attain 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}";
            }
            else if(lastPunchOut == null)
            {
                TimeSpan remainingTime = targetWorkTime - totalWorked;
                DateTime completionTime = DateTime.Now.Add(remainingTime);
                output = $"You will attain 8 hours at {completionTime.ToString("dd-MM-yyyy hh:mm:ss tt")}";
            }
              

            
            var result = new
            {
                error = false,
                status = true,
                message = "Punchin time",
                data = new
                {                   
                    CompletionTime = output
                }
            };

            return Ok(result);
        }
    }
}
