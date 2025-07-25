﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TimeCalculator.Interfaces;

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

            var punchTimes = _punchService.CreatePunchData(punchData);
            TimeSpan targetWorkTime = TimeSpan.FromHours(8);

            TimeSpan totalWorked = TimeSpan.Zero;

            foreach (var punchTime in punchTimes)
            {
                if (punchTime.PunchOut.HasValue)
                {
                    totalWorked += punchTime.PunchOut.Value.Subtract(punchTime.PunchIn);
                }
                else
                {
                    if (totalWorked.TotalHours < 8)
                    {
                        totalWorked += DateTime.Now.Subtract(punchTime.PunchIn);
                    }
                    else
                    {
                        totalWorked = totalWorked + TimeSpan.Zero;
                    }

                }
            }
            totalWorked = new TimeSpan(totalWorked.Hours, totalWorked.Minutes, totalWorked.Seconds);

            var lastPunchOut = punchTimes[punchTimes.Count - 1].PunchOut;
            var lastPunchIn = punchTimes[punchTimes.Count - 1].PunchIn;
            string? output = null;



            if (totalWorked >= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = totalWorked - targetWorkTime;
                var completedTime = Convert.ToDateTime(lastPunchOut).Subtract(workDifference);
                output = $"You have completed 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}. \n\n You have {workDifference.Hours} Hours ,{workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds as extra time";
            }
            else if (totalWorked > targetWorkTime && lastPunchOut == null)
            {
                var workDifference = totalWorked - targetWorkTime;
                var completedTime = DateTime.Now.Subtract(workDifference);
                output = $"You have completed 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}.\n\n You have {workDifference.Days} Days,{workDifference.Hours} Hours,{workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds as overTime";
            }

            else if (totalWorked <= targetWorkTime && lastPunchOut != null)
            {
                var workDifference = targetWorkTime - totalWorked;
                var completedTime = Convert.ToDateTime(lastPunchOut).Add(workDifference);
                output = $"You are {workDifference.Hours} Hours ,{workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds deficit for attaining 8 hours. \n\n You could have been attain 8 hours at {completedTime.ToString("dd-MM-yyyy hh:mm:ss tt")}";
            }
            else if (lastPunchOut == null)
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

        [HttpPost("caluclateV2")]
        public IActionResult CaluclateV2(int dayType, string punchData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(punchData))
                {
                    return BadRequest(new
                    {
                        error = true,
                        status = false,
                        message = "PunchData cannot be null or empty",
                        data = new { }
                    });
                }

                var punchTimes = _punchService.CreatePunchData(punchData);
                TimeSpan targetWorkTime = dayType == 1 ? TimeSpan.FromHours(4) : TimeSpan.FromHours(8);
                

                TimeSpan totalWorked = TimeSpan.Zero;

                foreach (var punchTime in punchTimes)
                {
                    if (punchTime.PunchOut.HasValue)
                    {
                        totalWorked += punchTime.PunchOut.Value.Subtract(punchTime.PunchIn);
                    }
                    else
                    {
                        if (totalWorked.Hours < targetWorkTime.Hours)
                        {
                            totalWorked += _punchService.GetIndianTime().Subtract(punchTime.PunchIn);
                        }
                        else
                        {
                            totalWorked = totalWorked + TimeSpan.Zero;
                        }
                    }
                }

                var lastPunchOut = punchTimes[punchTimes.Count - 1].PunchOut;
                var lastPunchIn = punchTimes[punchTimes.Count - 1].PunchIn;
                string? output = null;

                if (totalWorked.TotalSeconds >= targetWorkTime.TotalSeconds && lastPunchOut != null)
                {
                    var workDifference = totalWorked - targetWorkTime;
                    var completedTime = Convert.ToDateTime(lastPunchOut).Subtract(workDifference);
                    output = $"You have completed {targetWorkTime.Hours} hours at {completedTime:dd-MM-yyyy hh:mm:ss tt}. \n\nYou have {workDifference.Hours} Hours, {workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds as extra time";
                }
                else if (totalWorked.TotalSeconds > targetWorkTime.TotalSeconds && lastPunchOut == null)
                {
                    var workDifference = totalWorked - targetWorkTime;
                    var completedTime = _punchService.GetIndianTime().Subtract(workDifference);
                    output = $"You have completed {targetWorkTime.Hours} hours at {completedTime:dd-MM-yyyy hh:mm:ss tt}. \n\nYou have {workDifference.Days} Days, {workDifference.Hours} Hours, {workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds as overTime";
                }
                else if (totalWorked.TotalSeconds <= targetWorkTime.TotalSeconds && lastPunchOut != null)
                {
                    var workDifference = targetWorkTime - totalWorked;
                    var completedTime = Convert.ToDateTime(lastPunchOut).Add(workDifference);
                    output = $"You are {workDifference.Hours} Hours, {workDifference.Minutes} Minutes and {workDifference.Seconds} Seconds short of attaining {targetWorkTime.Hours} hours. \n\nYou could have attained it at {completedTime:dd-MM-yyyy hh:mm:ss tt}";
                }
                else if (lastPunchOut == null)
                {
                    TimeSpan remainingTime = targetWorkTime - totalWorked;
                    DateTime completionTime = _punchService.GetIndianTime().Add(remainingTime);
                    output = $"You will attain {targetWorkTime.Hours} hours at {completionTime:dd-MM-yyyy hh:mm:ss tt}";
                }

                return Ok(new
                {
                    error = false,
                    status = true,
                    message = "Punch-in time",
                    data = new
                    {
                        CompletionTime = output,
                        TotalWorked = totalWorked,
                        TargetWorkTime = targetWorkTime,
                        ServerTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"),
                        IndianTime = _punchService.GetIndianTime().ToString("dd-MM-yyyy hh:mm:ss tt")
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = true,
                    status = false,
                    message = ex.Message,
                    data = new { }
                });
            }
        }

    }
}
