using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace API.Helpers.RoleClaimTransformers;

public class KeyCloakRoleClaimsTransformation: IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity == null || !principal.Identity.IsAuthenticated)
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        var identity = (ClaimsIdentity)principal.Identity;
        var resourceAccess = identity.FindFirst("resource_access");
        if (resourceAccess == null || string.IsNullOrWhiteSpace(resourceAccess.Value))
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        var resources = JsonSerializer.Deserialize<Dictionary<string, Resource>>(resourceAccess.Value);
        if (resources == null || resources.Count == 0)
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        var agoraApi = resources.GetValueOrDefault("agora");
        if (agoraApi?.roles == null)
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        foreach (var role in agoraApi.roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return Task.FromResult<ClaimsPrincipal>(principal);
    }

    private class Resource
    {
        public List<string>? roles { get; set; }
    }

}