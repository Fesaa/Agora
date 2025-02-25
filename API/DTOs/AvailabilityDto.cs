using System;

namespace API.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string TimeRange { get; set; }
}