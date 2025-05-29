using System.Collections.Generic;

namespace API.DTOs;

public class MeetingRoomDto
{
    public int Id { get; set; }
    
    public required string DisplayName { get; set; }
    
    public required string Description { get; set; }
    
    public required string Location { get; set; }
    
    public required bool RequiresAck { get; set; }
    
    public required int Capacity { get; set; }
    
    public bool MayExceedCapacity { get; set; } = false;
    
    public bool MergeAble { get; set; } = false;
    
    public List<FaclitiyDto> Facilities { get; set; } = [];
    
    public List<MergeRoomDto> MergeRooms { get; set; } = [];
    
    
}