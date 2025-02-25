using System;

namespace API.Entities;

public class Availability
{
    public int Id { get; set; }
    public required DayOfWeek DayOfWeek { get; set; }
    /// <summary>
    /// Which hours of the day this facility is available
    /// </summary>
    /// <remarks>Type is a placeholder, till we further work this feature out</remarks>
    public string TimeRange { get; set; } = string.Empty;
    
}