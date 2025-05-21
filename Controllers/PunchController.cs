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
            TimeSpan targetWorkTime = TimeSpan.FromHours(8);

            TimeSpan totalWorked = TimeSpan.Zero;

            TimeSpan overTime = TimeSpan.Zero;

            foreach (var punchTime in punchTimes)
            {
                if (punchTime.PunchOut.HasValue)
                {
                    totalWorked += punchTime.PunchOut.Value - punchTime.PunchIn;
                }
                else
                {
                    if(totalWorked.TotalHours < 8)
                    {
                        totalWorked += DateTime.Now - punchTime.PunchIn;
                    }
                    else
                    {
                       totalWorked = totalWorked + TimeSpan.Zero;
                       overTime = DateTime.Now - punchTime.PunchIn;
                    }
                    
                }
            }
            totalWorked = new TimeSpan(totalWorked.Hours, totalWorked.Minutes, totalWorked.Seconds);

            var lastPunchOut = punchTimes[punchTimes.Count - 1].PunchOut;
            var lastPunchIn = punchTimes[punchTimes.Count - 1].PunchIn;
            string? output = null;

          

            if(totalWorked >= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = totalWorked - targetWorkTime;              
                var completedTime = Convert.ToDateTime(lastPunchOut) - workDifference;
                output = $"You have completed 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}. \n\n You have {workDifference.Hours} Hours ,{workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds as extra time";
            }
            else if(totalWorked > targetWorkTime && lastPunchOut == null)
            {
                var workDifference = totalWorked - targetWorkTime;
                var completedTime = Convert.ToDateTime(lastPunchIn) - workDifference;
                output = $"You have completed 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}.\n\n You have {overTime.Days} Days,{overTime.Hours} Hours,{overTime.Minutes} Minutes and {overTime.Seconds} Seconds as overTime";
            }

            else if(totalWorked <= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = targetWorkTime - totalWorked;
                var completedTime = Convert.ToDateTime(lastPunchOut) + workDifference;
                output = $"You are {workDifference.Hours} Hours ,{workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds deficit for attaining 8 hours. \n\n You could have been attain 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}";
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
