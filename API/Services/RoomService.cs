using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Services;

public interface IRoomService
{
    
    Task<MeetingRoom> Create(MeetingRoomDto meetingRoomDto);
    Task<MergeRooms> CreateMergeRoom(MergeRoomDto mergeRoomDto);
    
    Task<MeetingRoom> Update(MeetingRoomDto meetingRoomDto);
    Task<MergeRooms> UpdateMergeRoom(MergeRoomDto mergeRoomDto);
    Task Delete(int id);
    Task DeleteMergeRoom(int id);
}

public class RoomService(ILogger<RoomService> logger, IUnitOfWork unitOfWork, IMapper mapper, DataContext dataContext): IRoomService
{
    
    public async Task<MeetingRoom> Create(MeetingRoomDto meetingRoomDto)
    {
        if (meetingRoomDto.MergeRooms.Count != 0)
        {
            throw new AgoraException("merge-rooms-added-separately");
        }
        
        var room = await unitOfWork.RoomRepository.GetMeetingRoomByName(meetingRoomDto.DisplayName);
        if (room != null)
        {
            throw new AgoraException("name-already-used");
        }

        room = new MeetingRoom()
        {
            Id = meetingRoomDto.Id,
            DisplayName = meetingRoomDto.DisplayName,
            Location = meetingRoomDto.Location,
            Capacity = meetingRoomDto.Capacity,
            Description = string.IsNullOrEmpty(meetingRoomDto.Description) ? string.Empty : meetingRoomDto.Description,
            MergeAble = meetingRoomDto.MergeAble,
            MayExceedCapacity = meetingRoomDto.MayExceedCapacity,
        };
        await ValidateFacilities(room);

        logger.LogDebug("Creating a new room {Name} on {Location}", room.DisplayName, room.Location);
        unitOfWork.RoomRepository.Add(room);

        if (unitOfWork.HasChanges())
        {
            await unitOfWork.CommitAsync();
        }

        return room;
    }
    public Task<MergeRooms> CreateMergeRoom(MergeRoomDto mergeRoomDto)
    {
        throw new System.NotImplementedException();
    }
    public async Task<MeetingRoom> Update(MeetingRoomDto meetingRoomDto)
    {
        var room = await unitOfWork.RoomRepository.GetMeetingRoom(meetingRoomDto.Id);
        if (room == null)
        {
            throw new AgoraException("room-not-found");
        }

        if (room.DisplayName != meetingRoomDto.DisplayName)
        {
            var other = await unitOfWork.RoomRepository.GetMeetingRoomByName(meetingRoomDto.DisplayName);
            if (other != null)
            {
                throw new AgoraException("name-already-used");
            }
        }

        if (!meetingRoomDto.MergeAble)
        {
            meetingRoomDto.MergeRooms.Clear();
        }

        
        room.DisplayName = meetingRoomDto.DisplayName;
        room.Location = meetingRoomDto.Location;
        room.Capacity = meetingRoomDto.Capacity;
        room.Description = meetingRoomDto.Description;
        room.MergeAble = meetingRoomDto.MergeAble;
        room.MayExceedCapacity = meetingRoomDto.MayExceedCapacity;
        
        var ids = meetingRoomDto.Facilities.Select(f => f.Id);
        var facilities = dataContext.Facilities.Where(f => ids.Contains(f.Id));
        room.Facilities.Clear();
        room.Facilities.AddRange(facilities);

        await ValidateFacilities(room);
        
        unitOfWork.RoomRepository.Update(room);

        if (unitOfWork.HasChanges())
        {
            await unitOfWork.CommitAsync();
        }
        
        return room;
    }
    public Task<MergeRooms> UpdateMergeRoom(MergeRoomDto mergeRoomDto)
    {
        throw new System.NotImplementedException();
    }
    public Task Delete(int id)
    {
        throw new System.NotImplementedException();
    }
    public Task DeleteMergeRoom(int id)
    {
        throw new System.NotImplementedException();
    }

    private async Task ValidateFacilities(MeetingRoom meetingRoom)
    {
        // TODO: Validate we're not removing any that are still in use. bad naming have to rework. No time for now. Meetings don't exist yet anyway
    }
}