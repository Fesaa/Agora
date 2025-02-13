using System.Collections.Generic;

namespace API.Entities;

public class MergeRooms
{

    public int Id { get; set; }

    /// <summary>
    /// The meeting room that represents the full room
    /// </summary>
    public int ParentRoomId { get; set; }
    public required MeetingRoom Parent { get; set; }

    /// <summary>
    /// The meeting room(s) that make up the parent
    /// </summary>
    public List<MeetingRoom> MeetingRooms { get; } = [];
}