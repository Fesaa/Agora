using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

[Flags]
public enum MeetingIncludes
{
    None = 0,
    Room = 1 << 1,
    Facilities = 1 << 2,
}

public delegate IQueryable<Meeting> MeetingQueryOption(IQueryable<Meeting> query);

public interface IMeetingRepository
{
    void Add(Meeting meeting);
    void Update(Meeting meeting);
    void Remove(Meeting meeting);
    Task<IEnumerable<MeetingDto>> GetMeetingDtos(params MeetingQueryOption[] options);
    Task<IEnumerable<Meeting>> GetMeetings(params MeetingQueryOption[] options);
    Task<Meeting?> GetMeetingById(int id, MeetingIncludes include = MeetingIncludes.Room);
}

public class MeetingRepository(DataContext context, IMapper mapper): IMeetingRepository
{

    public void Add(Meeting meeting)
    {
        context.Meetings.Add(meeting);
    }
    public void Update(Meeting meeting)
    {
        context.Meetings.Update(meeting).State = EntityState.Modified;
    }
    public void Remove(Meeting meeting)
    {
        context.Meetings.Remove(meeting);
    }
    public async Task<IEnumerable<MeetingDto>> GetMeetingDtos(params MeetingQueryOption[] options)
    {
        var q = context.Meetings.AsQueryable()
            .AsNoTracking();

        foreach (var option in options)
        {
            q = option(q);
        }
        
        return await mapper.ProjectTo<MeetingDto>(q).ToListAsync();
    }
    public async Task<IEnumerable<Meeting>> GetMeetings(params MeetingQueryOption[] options)
    {
        var q = context.Meetings.AsQueryable()
            .AsNoTracking();

        foreach (var option in options)
        {
            q = option(q);
        }

        return await q.ToListAsync();
    }
    public async Task<Meeting?> GetMeetingById(int id, MeetingIncludes include = MeetingIncludes.Room)
    {
        return await context.Meetings
            .Includes(include)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public static MeetingQueryOption OnDate(DateTime date)
    {
        return q => q.Where(m => m.StartTime.Date == date.Date);
    }

    public static MeetingQueryOption StartAfter(DateTime after)
    {
        return q => q.Where(m => m.StartTime >= after);
    }

    public static MeetingQueryOption StartBefore(DateTime before)
    {
        return q => q.Where(m => m.StartTime <= before);
    }

    public static MeetingQueryOption EndAfter(DateTime after)
    {
        return q => q.Where(m => m.EndTime >= after);
    }

    public static MeetingQueryOption EndBefore(DateTime before)
    {
        return q => q.Where(m => m.EndTime <= before);
    }

    public static MeetingQueryOption StartBetween(DateTime before, DateTime after)
    {
        return q => q.Where(m => m.StartTime >= before && m.StartTime <= after);
    }

    public static MeetingQueryOption EndBetween(DateTime before, DateTime after)
    {
        return q => q.Where(m => m.EndTime >= before && m.EndTime <= after);
    }

    public static MeetingQueryOption InRoom(int roomId)
    {
        return q => q.Where(m => m.Room.Id == roomId);
    }

    /// <summary>
    /// Filters on the room id, or rooms where one of its children has the id
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public static MeetingQueryOption InRoomOrMergedRoom(int roomId)
    {
        return q => 
            q.Where(m => m.Room.Id == roomId || 
                         m.Room.ParentMergeRooms
                             .Any(r => r.MeetingRooms.Select(mm => mm.Id)
                                 .Contains(roomId)));
    }

    public static MeetingQueryOption InAnyRoom(IEnumerable<int> roomIds)
    {
        return q => q.Where(m => roomIds.Contains(m.Room.Id));
    }
    
    public static MeetingQueryOption IsAttending(string userId)
    {
        return q => q.Where(m => m.Attendees.Contains(userId) || m.CreatorId == userId);
    }

    public static MeetingQueryOption IsUsing(int facilityId)
    {
        return q => q.Where(m => m.UsedFacilities.Select(f => f.Id).Contains(facilityId));
    }

    public static MeetingQueryOption WithRoom()
    {
        return q => q.Include(m => m.Room);
    }
}