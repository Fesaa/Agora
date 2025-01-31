using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace API.Helpers.RoleClaimTransformers;

public class AzureRoleClaimsTransformation: IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity == null || !principal.Identity.IsAuthenticated)
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        var identity = (ClaimsIdentity)principal.Identity;
        var rolesClaim = identity.FindFirst("roles");
        if (rolesClaim == null)
        {
            rolesClaim = identity.FindFirst(ClaimTypes.Role);
        }

        if (rolesClaim == null || string.IsNullOrWhiteSpace(rolesClaim.Value))
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        var roles = JsonSerializer.Deserialize<List<string>>(rolesClaim.Value);
        if (roles == null || roles.Count == 0)
        {
            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return Task.FromResult<ClaimsPrincipal>(principal);
    }
}