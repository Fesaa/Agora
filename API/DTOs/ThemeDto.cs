using API.Entities.Enums;

namespace API.DTOs;

public class ThemeDto
{
    public int Id { get; set; }
    /// <summary>
    /// Name to display in the UI
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The .css file to use
    /// </summary>
    public required string FileName { get; set; }

    public required Provider ThemeProvider { get; set; } = Provider.System;

    public string Selector => "bg-" + Name;
}