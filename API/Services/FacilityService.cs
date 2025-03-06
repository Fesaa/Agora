using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
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
    Task DeleteAsync(int id);
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
    public async Task DeleteAsync(int id)
    {
        var f = await unitOfWork.FacilityRepository.GetById(id);
        if (f == null)
        {
            throw new AgoraException("facility-not-found");
        }
        
        unitOfWork.FacilityRepository.Delete(f);
    }
}