using System;
using System.Collections.Generic;

namespace API.Entities;

public class Meeting
{
    public int Id { get; set; }
    
    public required string CreatorId { get; set; }
    
    public required string Title { get; set; }
    
    public string Description { get; set; } = String.Empty;

    /// <summary>
    /// ID to communicate with Microsoft / Google calendar 
    /// </summary>
    public string ExternalId { get; set; } = "";
    
    /// <summary>
    /// UTC
    /// </summary>
    public required DateTime StartTime { get; set; }
    
    /// <summary>
    /// UTC
    /// </summary>
    public required DateTime EndTime { get; set; }

    /// <summary>
    /// Is true if the room does not require it
    /// </summary>
    public required bool Acknowledged { get; set; } = true;
    
    /// <summary>
    /// The room this meeting is happening in
    /// </summary>
    public required MeetingRoom Room { get; set; }
    
    /// <summary>
    /// The IDs provided by OpenIdConnect for each attendee
    /// </summary>
    public IList<string> Attendees { get; set; } = [];
    
    /// <summary>
    /// The facilities which the meeting will be using
    /// </summary>
    public IList<Facility> UsedFacilities { get; set; } = [];
}