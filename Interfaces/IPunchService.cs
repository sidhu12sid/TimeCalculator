using TimeCalculator.Models;

namespace TimeCalculator.Interfaces
{
    public interface IPunchService
    {
        List<PunchModel> CreatePunchData(string input);
        DateTime GetIndianTime();
    }
}
