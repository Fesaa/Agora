using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface IFacilityRepository
{
    Task<List<Facility>> All();
    Task<List<FaclitiyDto>> AllDtos(bool activeOnly);
    Task<Facility?> GetById(int id);
    
    void Add(Facility facility);
    void Update(Facility facility);
    void Delete(Facility facility);
}

public class FacilityRepository(DataContext context, IMapper mapper): IFacilityRepository
{

    public async Task<List<Facility>> All()
    {
        return await context.Facilities
            .ToListAsync();
    }
    public async Task<List<FaclitiyDto>> AllDtos(bool activeOnly)
    {
        return await mapper
            .ProjectTo<FaclitiyDto>(context.Facilities.Where(f => !activeOnly || f.Active))
            .ToListAsync();
    }
    public async Task<Facility?> GetById(int id)
    {
        return await context.Facilities
            .FirstOrDefaultAsync(f => f.Id == id);
    }
    public void Add(Facility facility)
    {
        context.Facilities.Add(facility);
    }
    public void Update(Facility facility)
    {
        context.Facilities.Update(facility).State = EntityState.Modified;
    }
    public void Delete(Facility facility)
    {
        context.Facilities.Remove(facility);
    }
}