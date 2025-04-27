using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface IRoomRepository
{
    Task<MeetingRoom?> GetMeetingRoom(int id);
    Task<MeetingRoom?> GetMeetingRoomByName(string name);
    Task<MergeRooms?> GetMergeRooms(int id);
    
    Task<List<MeetingRoomDto>> GetMeetingRooms();
    Task<List<MeetingRoomDto>> GetMeetingRoomsAvailableOn(DateTime start, DateTime end);
    Task<List<MergeRoomDto>> GetMergeRooms();
    
    void Add(MeetingRoom meetingRoom);
    void Update(MeetingRoom meetingRoom);
    void Remove(MeetingRoom meetingRoom);
}

public class RoomRepository(DataContext context, IMapper mapper): IRoomRepository
{

    public async Task<MeetingRoom?> GetMeetingRoom(int id)
    {
        return await context.MeetingRooms
            .Include(m => m.Facilities)
            .Where(mr => mr.Id == id)
            .FirstOrDefaultAsync();
    }
    public async Task<MeetingRoom?> GetMeetingRoomByName(string name)
    {
        return await context.MeetingRooms
            .Where(mr => mr.DisplayName == name)
            .FirstOrDefaultAsync();
    }
    public async Task<MergeRooms?> GetMergeRooms(int id)
    {
        return await context.MergeRooms
            .Where(mr => mr.Id == id)
            .FirstOrDefaultAsync();
    }
    public async Task<List<MeetingRoomDto>> GetMeetingRooms()
    {
        return await mapper
            .ProjectTo<MeetingRoomDto>(context.MeetingRooms)
            .ToListAsync();
    }
    public async Task<List<MeetingRoomDto>> GetMeetingRoomsAvailableOn(DateTime start, DateTime end)
    {
        return await mapper
            .ProjectTo<MeetingRoomDto>(context.MeetingRooms
                .Where(r => !r.Meetings.Any(m => 
                    m.StartTime <= end && m.EndTime >= start))
            )
            .ToListAsync();
    }
    public async Task<List<MergeRoomDto>> GetMergeRooms()
    {
        return await mapper
            .ProjectTo<MergeRoomDto>(context.MergeRooms)
            .ToListAsync();
    }
    public void Add(MeetingRoom meetingRoom)
    {
        context.MeetingRooms.Add(meetingRoom);
    }
    public void Update(MeetingRoom meetingRoom)
    {
        context.MeetingRooms.Update(meetingRoom).State = EntityState.Modified;
    }
    public void Remove(MeetingRoom meetingRoom)
    {
        context.MeetingRooms.Remove(meetingRoom);
    }
}