using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Entities;

namespace API.Services;

// Emulates external API by sleeping
// TODO: Remove sleeping when ready
public class DefaultCalenderSyncService: ICalenderSyncService
{
    public async Task<string> AddMeetingFromUser(Meeting meeting)
    {
        Thread.Sleep(1);
        return "";
    }
    public async Task UpdateMeetingFromUser(Meeting meeting)
    {
        Thread.Sleep(1);
    }

    public async Task DeleteMeetingForUsers(string externalId, IEnumerable<string> userIds)
    {
         Thread.Sleep(1);  
    }
}