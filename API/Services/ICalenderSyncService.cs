using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;

namespace API.Services;

public interface ICalenderSyncService
{
    Task<string> AddMeetingFromUser(Meeting meeting);
    Task UpdateMeetingFromUser(Meeting meeting);
    Task DeleteMeetingForUsers(string externalId, IEnumerable<string> userIds);
}