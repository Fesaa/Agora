using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Entities;
using API.Exceptions;
using Microsoft.Extensions.Logging;

namespace API.Services;

public interface IMeetingService
{
    Task<bool> IsAvailable(MeetingDto meetingDto);
    Task CreateMeeting(string userId, MeetingDto meetingDto);
    Task UpdateMeeting(MeetingDto meetingDto);
    Task DeleteMeeting(int meetingId);
    Task<IEnumerable<MeetingSlot>> AvailableSlotsForOn(int roomId, DateTime date);
}

public class MeetingService(ILogger<MeetingService> logger, IUnitOfWork unitOfWork,
    ICalenderSyncService calenderSyncService, DataContext context): IMeetingService
{

    public async Task<bool> IsAvailable(MeetingDto meetingDto)
    {
        if (meetingDto.MeetingRoom == null)
        {
            throw new AgoraException("meeting-needs-room");
        }
        
        // TODO: Someone else has to check if this is correct!
        var overlapping = await unitOfWork.MeetingRepository.GetMeetingDtos(
            MeetingRepository.InRoomOrMergedRoom(meetingDto.MeetingRoom.Id),
            MeetingRepository.StartBefore(meetingDto.EndTime.ToUniversalTime()),
            MeetingRepository.EndAfter(meetingDto.StartTime.ToUniversalTime()));
        
        return !overlapping.Any();
    }
    public async Task CreateMeeting(string userId, MeetingDto meetingDto)
    {
        if (meetingDto.MeetingRoom == null)
        {
            throw new AgoraException("meeting-needs-room");
        }
        
        if (!await IsAvailable(meetingDto))
        {
            throw new AgoraException("meeting-overlap");
        }
        
        var room = await unitOfWork.RoomRepository.GetMeetingRoom(meetingDto.MeetingRoom.Id);
        if (room == null)
        {
            throw new AgoraException("room-not-found");
        }
        
        var facilities = await unitOfWork.FacilityRepository
            .GetByIdsInRoom(meetingDto.Facilities.Select(f => f.Id).ToList(), room.Id, true);

        if (facilities.Count != meetingDto.Facilities.Count)
        {
            logger.LogWarning("Meeting by {UserId} on {Date} in {Room} requested unknown facilities {amount} ",
                userId, meetingDto.StartTime.Date, meetingDto.MeetingRoom.DisplayName, meetingDto.Facilities.Count - facilities.Count);
        }

        var meeting = new Meeting
        {
            CreatorId = userId,
            Room = room,
            Title = meetingDto.Title,
            Description = meetingDto.Description,
            StartTime = meetingDto.StartTime.ToUniversalTime(),
            EndTime = meetingDto.EndTime.ToUniversalTime(),
            Attendees = await SanitizedAttendees(meetingDto.Attendees),
            UsedFacilities = facilities,
        };

        // TODO: Alert management if required (facilities)

        if (meeting.Attendees.Count == 0 && meetingDto.Attendees.Any())
        {
            logger.LogWarning("Meeting by {UserId} on {Date} in {Room} has no valid attendees. Hoped for {AttendeesCount}!",
                userId, meeting.StartTime.Date, meeting.Room.DisplayName, meetingDto.Attendees.Count);
        }
        
        meeting.ExternalId = await calenderSyncService.AddMeetingFromUser(meeting);
        unitOfWork.MeetingRepository.Add(meeting);
        await unitOfWork.CommitAsync();
    }
    public async Task UpdateMeeting(MeetingDto meetingDto)
    {
        var meeting = await unitOfWork.MeetingRepository.GetMeetingById(meetingDto.Id);
        if (meeting == null)
        {
            throw new AgoraException("meeting-not-found");
        }

        if (!string.IsNullOrEmpty(meetingDto.Title))
        {
            meeting.Title = meetingDto.Title;
        }

        // Description is allowed to be empty
        meeting.Description = meetingDto.Description;
        
        meeting.StartTime = meetingDto.StartTime.ToUniversalTime();
        meeting.EndTime = meetingDto.EndTime.ToUniversalTime();

        // Only update the room if one is present, a meeting needs a room
        if (meetingDto.MeetingRoom != null && meeting.Room.Id != meetingDto.MeetingRoom.Id)
        {
            var room = await unitOfWork.RoomRepository.GetMeetingRoom(meetingDto.MeetingRoom.Id);
            if (room == null)
            {
                throw new AgoraException("room-not-found");
            }
            
            meeting.Room = room;
        }

        // Update attendees
        var toDeleteFor = meeting.Attendees.
            Where(a => !meetingDto.Attendees.Contains(a)).
            ToList();

        meeting.Attendees = await SanitizedAttendees(meetingDto.Attendees);
        if (meeting.Attendees.Count == 0 && meetingDto.Attendees.Any())
        {
            logger.LogWarning("Meeting {Id} on {Date} in {Room} has no valid attendees. Hoped for {AttendeesCount}!",
                meeting.Id, meeting.StartTime.Date, meeting.Room.DisplayName, meetingDto.Attendees.Count);
        }

        // Update facilities
        var facilities = await unitOfWork.FacilityRepository
            .GetByIdsInRoom(meetingDto.Facilities.Select(f => f.Id).ToList(), meeting.Room.Id, true);
        if (facilities.Count != meetingDto.Facilities.Count)
        {
            logger.LogWarning("Meeting by {MeetingId} on {Date} in {Room} requested unknown facilities {amount} ",
                meeting.Id, meetingDto.StartTime.Date, meeting.Room.DisplayName, meetingDto.Facilities.Count - facilities.Count);
        }

        meeting.UsedFacilities = facilities;
        // TODO: Alert management if required (facilities)

        // Sync external calenders & save to local database
        // TODO: Try catch? How do we want to handle failures to external API's?
        await calenderSyncService.DeleteMeetingForUsers(meeting.ExternalId, toDeleteFor); // Is this needed?
        await calenderSyncService.UpdateMeetingFromUser(meeting);
        
        unitOfWork.MeetingRepository.Update(meeting);
        await unitOfWork.CommitAsync();
    }
    public async Task DeleteMeeting(int meetingId)
    {
        var meeting = await unitOfWork.MeetingRepository.GetMeetingById(meetingId);
        if (meeting == null)
        {
            throw new AgoraException("meeting-not-found");
        }

        // TODO: Should we allow deleting past meetings?

        var allUsers = meeting.Attendees;
        allUsers.Add(meeting.CreatorId);
        await calenderSyncService.DeleteMeetingForUsers(meeting.ExternalId, allUsers);
        
        unitOfWork.MeetingRepository.Remove(meeting);
        await unitOfWork.CommitAsync();
    }
    public async Task<IEnumerable<MeetingSlot>> AvailableSlotsForOn(int roomId, DateTime date)
    {
        // TODO: Implement proper slotting
        /*var meetings = await unitOfWork.MeetingRepository.GetMeetings(
            MeetingRepository.InRoomOrMergedRoom(roomId),
            MeetingRepository.OnDate(date));
        
        var slots = new List<MeetingSlot>();
        
        foreach (var meeting in meetings)
        {
            var last = slots.LastOrDefault()?.End ?? date.Date;
            
            slots.Add(new MeetingSlot()
            {
                Start = last,
                End = meeting.StartTime,
            });
        }
        
        // And rest of the day
        var lastSlotEnding = slots.LastOrDefault()?.End ?? date.Date;
        if (lastSlotEnding < date.AddDays(1).Date)
        {
            slots.Add(new MeetingSlot()
            {
                Start = lastSlotEnding,
                End = date.AddDays(1).Date,
            });
        }
        
        
        return slots;*/
        // Return the entire day for now
        logger.LogInformation("Trying for RoomId {RoomId} on {Date}", roomId, date);
        return
        [
            new MeetingSlot()
            {
              Start = date.Date,
              End = date.AddDays(1).Date,
            },
        ];
    }

    // TODO: Allow non company users to be invited, save email for them and external id for others?
    private async Task<IList<string>> SanitizedAttendees(IList<string> attendees)
    {
        return attendees;
        /*return await context.UserEmails
            .Where(e => attendees.Contains(e.ExternalId))
            .Select(e => e.ExternalId)
            .ToListAsync();*/
    }
}