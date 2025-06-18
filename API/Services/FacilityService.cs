using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Entities;
using API.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Services;

public interface IFacilityService
{
    Task<Facility> CreateAsync(FaclitiyDto facilityDto);
    Task<Facility> UpdateAsync(FaclitiyDto facilityDto);
    Task ActiveAsync(int id);
    // TODO: Change to Tuple with the specific meetings? Idk. Meetings aren't implemented yet. SO big TODO here.
    Task DeActivateAsync(int id);
    Task DeleteAsync(int id, bool force = false);
}

public class FacilityService(ILogger<FacilityService> logger, IUnitOfWork unitOfWork, IMapper mapper,
    INotifyService notifyService): IFacilityService
{

    public async Task<Facility> CreateAsync(FaclitiyDto facilityDto)
    {
        var f = new Facility()
        {
            DisplayName = facilityDto.DisplayName.Trim(),
            Description = facilityDto.Description.Trim(),
            Availability = facilityDto.Availability.Select(mapper.Map<Availability>).ToList(),
            Cost = facilityDto.Cost,
            AlertManagement = facilityDto.AlertManagement,
        };

        unitOfWork.FacilityRepository.Add(f);
        await unitOfWork.CommitAsync();

        return f;
    }
    public async Task<Facility> UpdateAsync(FaclitiyDto facilityDto)
    {
        var f = await unitOfWork.FacilityRepository.GetById(facilityDto.Id) ?? throw new AgoraException("facility-not-found");;
        
        f.DisplayName = facilityDto.DisplayName;
        f.Description = facilityDto.Description;
        
        // TODO: Remove from upcoming meetings that use this during unavailable hours/days
        // And send notification if so
        var existingAvailabilities = f.Availability.ToDictionary(a => a.Id);
        f.Availability.Clear();
        foreach (var mapped in facilityDto.Availability.Select(mapper.Map<Availability>))
        {
            if (existingAvailabilities.TryGetValue(mapped.Id, out var existing))
            {
                existing.DayOfWeek = mapped.DayOfWeek;
                existing.TimeRange = mapped.TimeRange;
                f.Availability.Add(existing);
            }
            else
            {
                f.Availability.Add(mapped);
            }
        }

        
        f.Cost = facilityDto.Cost;
        f.AlertManagement = facilityDto.AlertManagement;
        unitOfWork.FacilityRepository.Update(f);

        if (unitOfWork.HasChanges())
        {
            await unitOfWork.CommitAsync();
        }
        
        return f;
    }
    public async Task ActiveAsync(int id)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id) ?? throw new AgoraException("facility-not-found");;
        
        f.Active = true;
        unitOfWork.FacilityRepository.Update(f);
        await unitOfWork.CommitAsync();
    }
    public async Task DeActivateAsync(int id)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }
        
        f.Active = false;
        unitOfWork.FacilityRepository.Update(f);

        foreach (var room in f.MeetingRooms)
        {
            var meetings = await unitOfWork.MeetingRepository.GetMeetings(
                MeetingRepository.InRoomOrMergedRoom(room.Id),
                MeetingRepository.EndAfter(DateTime.UtcNow),
                MeetingRepository.IsUsing(id));
            
            room.Facilities.Remove(f);
            await NotifyFacilityLostForMeetings(f, meetings);
        }
        
        await unitOfWork.CommitAsync();
    }
    public async Task DeleteAsync(int id, bool force = false)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id) ?? throw new AgoraException("facility-not-found");

        var meetings = await unitOfWork.MeetingRepository.GetMeetings(
            MeetingRepository.InAnyRoom(f.MeetingRooms.Select(r => r.Id)),
            MeetingRepository.EndAfter(DateTime.UtcNow),
            MeetingRepository.IsUsing(id));

        if (meetings.Any() && !force) throw new AgoraException("facility-still-in-use");

        await NotifyFacilityLostForMeetings(f, meetings);
        foreach (var meeting in meetings)
        {
            meeting.UsedFacilities.Remove(f);
        }

        foreach (var room in f.MeetingRooms)
        {
            room.Facilities.Remove(f);
        }
        
        unitOfWork.FacilityRepository.Delete(f);
        await unitOfWork.CommitAsync();
    }

    /// <summary>
    /// Notifies all impacted organizers that they'll be missing a facility in an upcoming meeting
    /// </summary>
    /// <param name="facility"></param>
    /// <param name="meetings"></param>
    private async Task NotifyFacilityLostForMeetings(Facility facility, IList<Meeting> meetings)
    {
        if (!meetings.Any()) return;

        //var userIds = meeting.Attendees.Select(a => a).ToList();
        //userIds.Add(meeting.CreatorId);
        // TODO: Add actual information to notify, is NOP atm
        //await notifyService.Notify(userIds);
    }
    
}