using API.Entities.Enums;

namespace API.DTOs;

public class OpenIdConnectInfoDto
{
    public required string Authority { get; set; } = null!;
    public required OpenIdProvider Provider { get; set; }
}