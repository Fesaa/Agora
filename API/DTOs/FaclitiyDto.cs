using System.Collections.Generic;

namespace API.DTOs;

public class FaclitiyDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool AlertManagement { get; set; }
    public float Cost { get; set; }
    public bool Active { get; set; }
    public List<AvailabilityDto> Availability { get; set; }
}