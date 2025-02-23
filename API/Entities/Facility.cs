using System;
using System.Collections.Generic;

namespace API.Entities;

public class Facility
{
    public int Id { get; set; }

    /// <summary>
    /// The name of the facility provided
    /// </summary>
    /// <example>Lunch</example>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Extra information
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// If management (/reception) needs to notification if it has been selected. Defaults to false
    /// </summary>
    public bool AlertManagement { get; set; } = false;

    /// <summary>
    /// Optional additional cost, defaults to 0
    /// </summary>
    public float Cost { get; set; } = 0f;

    public List<Availability> Availability { get; set; } = [];

    /// <summary>
    /// The rooms this facility is used in
    /// </summary>
    public List<MeetingRoom> MeetingRooms { get; set; } = [];
}