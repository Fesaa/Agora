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

    public static MeetingFilterOption InAnyRoom(IEnumerable<int> roomIds)
    {
        return q => q.Where(m => roomIds.Contains(m.Room.Id));
    }
    
    public static MeetingFilterOption IsAttending(string userId)
    {
        return q => q.Where(m => m.Attendees.Contains(userId));
    }
}