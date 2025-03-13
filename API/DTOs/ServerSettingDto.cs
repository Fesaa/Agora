using API.Entities.Enums;

namespace API.DTOs;

public class ServerSettingDto
{
    
    public string? OpenIdAuthority { get; set; }
    public OpenIdProvider OpenIdProvider { get; set; }
    
    public CalenderSyncProvider CalenderSyncProvider { get; set; }
    
    public string LoggingLevel { get; set; } = "Information";
    
}