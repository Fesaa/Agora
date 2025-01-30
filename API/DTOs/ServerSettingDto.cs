namespace API.DTOs;

public class ServerSettingDto
{
    
    public string? OpenIdAuthority { get; set; }
    
    public string LoggingLevel { get; set; } = "Information";
    
}