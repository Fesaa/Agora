using System.Threading;
using System.Threading.Tasks;
using API.Entities;

namespace API.Services;

public interface ICalenderSyncService
{
    Task<string> AddMeetingFromUser(string userId, Meeting meeting);
}

public class DefaultCalenderSyncService: ICalenderSyncService
{
    public async Task<string> AddMeetingFromUser(string userId, Meeting meeting)
    {
        Thread.Sleep(1);
        return "";
    }
}