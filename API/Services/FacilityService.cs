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
    Task<IList<MeetingRoom>> DeActivateAsync(int id);
    Task DeleteAsync(int id, bool force = false);
}

public class FacilityService(ILogger<FacilityService> logger, IUnitOfWork unitOfWork, IMapper mapper): IFacilityService
{

    public async Task<Facility> CreateAsync(FaclitiyDto facilityDto)
    {
        var f = new Facility()
        {
            DisplayName = facilityDto.DisplayName,
            Description = facilityDto.Description,
            Availability = facilityDto.Availability.Select(mapper.Map<Availability>).ToList(),
            Cost = facilityDto.Cost,
            AlertManagement = facilityDto.AlertManagement,
        };

        unitOfWork.FacilityRepository.Add(f);
        if (unitOfWork.HasChanges())
        {
            await unitOfWork.CommitAsync();
        }
        return f;
    }
    public async Task<Facility> UpdateAsync(FaclitiyDto facilityDto)
    {
        var f = await unitOfWork.FacilityRepository.GetById(facilityDto.Id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }
        
        f.DisplayName = facilityDto.DisplayName;
        f.Description = facilityDto.Description;
        
        // TODO: Remove from upcoming meetings that use this during unavailable hours/days
        // And send notification if so
        f.Availability = facilityDto.Availability.Select(mapper.Map<Availability>).ToList();
        
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
        var f = await unitOfWork.FacilityRepository.GetById(id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }
        
        f.Active = true;
        unitOfWork.FacilityRepository.Update(f);
        await unitOfWork.CommitAsync();
    }
    public async Task<IList<MeetingRoom>> DeActivateAsync(int id)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }
        
        f.Active = false;
        unitOfWork.FacilityRepository.Update(f);

        var impacted = new List<MeetingRoom>();
        foreach (var room in f.MeetingRooms)
        {
            room.Facilities.Remove(f);
            // TODO: Check if there are upcoming meetings
            // if (room.HasUpcomingMeeting())
            // {
            //     impacted.Add(room);
            // }
        }
        
        await unitOfWork.CommitAsync();
        return impacted;
    }
    public async Task DeleteAsync(int id, bool force = false)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }

        var meetings = await unitOfWork.MeetingRepository.GetMeetings(
            MeetingRepository.InAnyRoom(f.MeetingRooms.Select(r => r.Id)),
            MeetingRepository.EndAfter(DateTime.UtcNow));

        if (meetings.Any() && !force)
        {
            // TODO: Use other exception?
            throw new AgoraException("facility-still-in-use");
        }
        
        unitOfWork.FacilityRepository.Delete(f);
        if (unitOfWork.HasChanges())
        {
            await unitOfWork.CommitAsync();
        }
    }
}