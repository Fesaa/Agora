using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public delegate IQueryable<Meeting> MeetingFilterOption(IQueryable<Meeting> query);

public interface IMeetingRepository
{
    void Add(Meeting meeting);
    void Update(Meeting meeting);
    void Remove(Meeting meeting);
    Task<IEnumerable<MeetingDto>> GetMeetingDtos(params MeetingFilterOption[] options);
    Task<IEnumerable<Meeting>> GetMeetings(params MeetingFilterOption[] options);
    Task<Meeting?> GetMeetingById(int id);
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
    public async Task<IEnumerable<MeetingDto>> GetMeetingDtos(params MeetingFilterOption[] options)
    {
        var q = context.Meetings.AsQueryable()
            .AsNoTracking();

        foreach (var option in options)
        {
            q = option(q);
        }
        
        return await mapper.ProjectTo<MeetingDto>(q).ToListAsync();
    }
    public async Task<IEnumerable<Meeting>> GetMeetings(params MeetingFilterOption[] options)
    {
        var q = context.Meetings.AsQueryable()
            .AsNoTracking();

        foreach (var option in options)
        {
            q = option(q);
        }

        return await q.ToListAsync();
    }
    public async Task<Meeting?> GetMeetingById(int id)
    {
        return await context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
    }

    public static MeetingFilterOption OnDate(DateTime date)
    {
        return q => q.Where(m => m.StartTime.Date == date.Date);
    }

    public static MeetingFilterOption StartAfter(DateTime before)
    {
        return q => q.Where(m => m.StartTime >= before);
    }

    public static MeetingFilterOption StartBefore(DateTime after)
    {
        return q => q.Where(m => m.StartTime <= after);
    }

    public static MeetingFilterOption EndAfter(DateTime after)
    {
        return q => q.Where(m => m.EndTime >= after);
    }

    public static MeetingFilterOption EndBefore(DateTime before)
    {
        return q => q.Where(m => m.EndTime <= before);
    }

    public static MeetingFilterOption StartBetween(DateTime before, DateTime after)
    {
        return q => q.Where(m => m.StartTime >= before && m.StartTime <= after);
    }

    public static MeetingFilterOption EndBetween(DateTime before, DateTime after)
    {
        return q => q.Where(m => m.EndTime >= before && m.EndTime <= after);
    }

    public static MeetingFilterOption InRoom(int roomId)
    {
        return q => q.Where(m => m.Room.Id == roomId);
    }

    /// <summary>
    /// Filters on the room id, or rooms where one of its children has the id
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public static MeetingFilterOption InRoomOrMergedRoom(int roomId)
    {
        return q => 
            q.Where(m => m.Room.Id == roomId || 
                         m.Room.ParentMergeRooms
                             .Any(r => r.MeetingRooms.Select(mm => mm.Id)
                                 .Contains(roomId)));
    }

    public static MeetingFilterOption InAnyRoom(IEnumerable<int> roomIds)
    {
        return q => q.Where(m => roomIds.Contains(m.Room.Id));
    }
    
    public static MeetingFilterOption IsAttending(string userId)
    {
        return q => q.Where(m => m.Attendees.Contains(userId));
    }

    public static MeetingFilterOption IsUsing(int facilityId)
    {
        return q => q.Where(m => m.UsedFacilities.Select(f => f.Id).Contains(facilityId));
    }
}