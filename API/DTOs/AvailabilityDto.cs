using System;
using System.Collections.Generic;

namespace API.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public List<DayOfWeek> DayOfWeek { get; set; }
    public string TimeRange { get; set; }
}