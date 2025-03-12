using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Exceptions;
using Microsoft.Extensions.Logging;

namespace API.Services;

public interface IMeetingService
{
    Task<bool> IsAvailable(MeetingDto meeting);
    Task CreateMeeting(MeetingDto meeting);
}

public class MeetingService(ILogger<MeetingService> logger, IUnitOfWork unitOfWork): IMeetingService
{

    public async Task<bool> IsAvailable(MeetingDto meeting)
    {
        // TODO: Someone else has to check if this is correct!
        var overlapping = await unitOfWork.MeetingRepository.GetMeetingDtos(
            MeetingRepository.InRoom(meeting.MeetingRoom.Id),
            MeetingRepository.StartBefore(meeting.EndTime),
            MeetingRepository.EndAfter(meeting.StartTime));
        
        return !overlapping.Any();
    }
    public async Task CreateMeeting(MeetingDto meeting)
    {
        if (!await IsAvailable(meeting))
        {
            throw new AgoraException("meeting-overlap");
        }
    }
}