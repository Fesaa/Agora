using System.Collections.Generic;

namespace API.Entities;

public class MeetingRoom
{

    public int Id { get; set; }

    /// <summary>
    /// The name used to identify the room, instead of it's identification
    /// </summary>
    /// <example>Meeting room [Old Director Name] instead of B4.23A</example>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Optional extra information about the room
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The internal identification string
    /// </summary>
    /// <example>B4.23A</example>
    public required string Location { get; set; }

    /// <summary>
    /// How many people can be seated
    /// </summary>
    public required int Capacity { get; set; }

    /// <summary>
    /// If the capacity me be ignored when creating a meeting in this room. Defaults to false
    /// </summary>
    public bool MayExceedCapacity { get; set; } = false;

    /// <summary>
    /// If the room is merge-able with others. Defaults to false
    /// </summary>
    public bool MergeAble { get; set; } = false;

    /// <summary>
    /// The possible facilities this room has access to
    /// </summary>
    public List<Facility> Facilities { get; set; } = [];

    /// <summary>
    /// The merged rooms this room is a part of
    /// </summary>
    public List<MergeRooms> MergeRooms { get; } = [];

    /// <summary>
    /// The MergeRooms where this MeetingRoom is the Parent
    /// </summary>
    public List<MergeRooms> ParentMergeRooms { get; set; } = [];
    
    /// <summary>
    /// Meetings in this room
    /// </summary>
    public List<Meeting> Meetings { get; set; } = [];
}