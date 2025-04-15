using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class StatsController(ILogger<StatsController> logger, IMeetingService meetingService,
    IUnitOfWork unitOfWork, IRoomService roomService, ILocalizationService localizationService,
    DataContext context): BaseApiController
{

    [HttpGet("rooms")]
    public async Task<IEnumerable<StatsRecord>> MeetingRoomStats(int limit = 10)
    {
        if (limit > 100)
        {
            limit = 100;
        }
        
        return await context.Meetings
            .GroupBy(m => m.Room)
            .Select(group => new StatsRecord
            {
                Id = group.Key.Id,
                Name = group.Key.DisplayName,
                Value = group.Count()
            })
            .OrderByDescending(r => r.Value)
            .Take(limit)
            .ToListAsync();
    }
    
    [HttpGet("facilities")]
    public async Task<IEnumerable<StatsRecord>> FacilityStats(int limit = 10)
    {
        if (limit > 100)
        {
            limit = 100;
        }
        
        return await context.Meetings
            .SelectMany(m => m.UsedFacilities)
            .GroupBy(f => f)
            .Select(group => new StatsRecord
            {
                Id = group.Key.Id,
                Name = group.Key.DisplayName,
                Value = group.Count()
            })
            .OrderByDescending(r => r.Value)
            .Take(limit)
            .ToListAsync();
    }
    
}