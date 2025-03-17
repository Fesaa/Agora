using System;
using System.Collections.Generic;

namespace API.DTOs;

public class MeetingDto
{
    public int Id { get; set; }
    
    public required string CreatorId { get; set; }
    
    public required string Title { get; set; }
    
    public string Description { get; set; } = String.Empty;
    
    public string ExternalId { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public MeetingRoomDto? MeetingRoom { get; set; }
    
    public IList<string> Attendees { get; set; }
    
    public IList<FaclitiyDto> Facilities { get; set; }
}