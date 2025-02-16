using API.Entities.Enums;

namespace API.Entities;

public class Theme
{

    public static readonly string DefaultTheme = "default";
    
    public int Id { get; set; }
    /// <summary>
    /// Name to display in the UI
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The .css file to use
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// The user that uploaded the theme
    /// </summary>
    public string? Author { get; set; }

    public bool Default { get; set; }

    public required Provider ThemeProvider { get; set; } = Provider.System;

}