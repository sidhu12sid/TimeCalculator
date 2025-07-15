using System.Collections.Generic;
using System.Globalization;
using TimeCalculator.Interfaces;
using TimeCalculator.Models;
using TimeZoneConverter;

namespace TimeCalculator.Services
{
    public class PunchService : IPunchService
    {
        public List<PunchModel> CreatePunchData(string input)
        {
            List<PunchModel> punchData = new List<PunchModel>();
            try
            {
                string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    var l = line.Trim();
                    string[] parts = l.Split('\t');
                    if (parts.Length < 2) continue;

                    DateTime punchIn = DateTime.ParseExact($"{parts[0]} {parts[1]}", "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    DateTime? punchOut = null;
                    if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3]))
                    {
                        punchOut = DateTime.ParseExact($"{parts[2]} {parts[3]}", "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    }

                    punchData.Add(new PunchModel { PunchIn = punchIn, PunchOut = punchOut });
                }
            }
            catch
            {
                return null;
            }
            return punchData;
        }

        public DateTime GetIndianTime()
        {
            TimeZoneInfo istZone = TZConvert.GetTimeZoneInfo("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);
        }


    }
}
