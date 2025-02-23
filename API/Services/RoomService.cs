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

        room = await ToMeetingRoom(meetingRoomDto);

        logger.LogDebug("Creating a new room {Name} on {Location}", room.DisplayName, room.Location);
        unitOfWork.RoomRepository.Add(room);
        await unitOfWork.CommitAsync();

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

        var newRoom = await ToMeetingRoom(meetingRoomDto);
        unitOfWork.RoomRepository.Update(newRoom);
        await unitOfWork.CommitAsync();
        
        return newRoom;
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

    private async Task<MeetingRoom> ToMeetingRoom(MeetingRoomDto meetingRoomDto)
    {
        var room = new MeetingRoom()
        {
            Id = meetingRoomDto.Id,
            DisplayName = meetingRoomDto.DisplayName,
            Location = meetingRoomDto.Location,
            Capacity = meetingRoomDto.Capacity,
            Description = string.IsNullOrEmpty(meetingRoomDto.Description) ? string.Empty : meetingRoomDto.Description,
            MergeAble = meetingRoomDto.MergeAble,
            MayExceedCapacity = meetingRoomDto.MayExceedCapacity,
        };
        
        // TODO: Check Facilities
        
        return room;
    }
}