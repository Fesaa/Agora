using System.Linq;
using API.Data.Repositories;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class IncludeExtensions
{

    public static IQueryable<Meeting> Includes(this IQueryable<Meeting> query, MeetingIncludes includes)
    {
        if (includes.HasFlag(MeetingIncludes.Room))
        {
            query = query.Include(m => m.Room);
        }

        if (includes.HasFlag(MeetingIncludes.Facilities))
        {
            query = query.Include(m => m.UsedFacilities);
        }
        
        return query;
    }
    
}