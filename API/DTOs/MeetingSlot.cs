using System;

namespace API.DTOs;

public class MeetingSlot
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
}