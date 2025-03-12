using System;
using System.Collections.Generic;

namespace API.DTOs;

public class MeetingDto
{
    public int Id { get; set; }
    
    public string ExternalId { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public MeetingRoomDto MeetingRoom { get; set; }
    
    public IList<string> Attendees { get; set; }
}