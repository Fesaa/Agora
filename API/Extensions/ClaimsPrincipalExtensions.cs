using System;
using System.Linq;
using System.Security.Claims;
using API.Constants;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{

    /// <summary>
    /// Get the name associated by the authenticated user
    /// </summary>
    /// <remarks>This name is returned by a 3rd party, do NOT assume it is safe to use as input</remarks>
    /// <param name="principal"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static string GetName(this ClaimsPrincipal principal)
    {
        var nameClaim = principal.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == ClaimTypes.Name);
        if (nameClaim == null)
        {
            throw new UnauthorizedAccessException();
        }

        return nameClaim.Value;
    }

    /// <summary>
    /// Returns the users identifier, this should be used to obtain users preferences, and other user related stuff
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static string GetIdentifier(this ClaimsPrincipal user)
    {
        var idClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        return idClaim.Value;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        if (emailClaim == null)
        {
            throw new UnauthorizedAccessException();
        }
        return emailClaim.Value;
    }

    public static bool HasPolicyClaim(this ClaimsPrincipal principal, string claim)
    {
        var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        foreach (var roleClaim in roleClaims)
        {
            if (roleClaim.Value.ToLower().Equals(claim.ToLower()))
            {
                return true;
            }
        }
        
        return false;
    }

    public static bool HasPolicyClaimOrAdmin(this ClaimsPrincipal principal, string claim)
    {
        var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        foreach (var roleClaim in roleClaims)
        {
            if (roleClaim.Value.ToLower().Equals(claim.ToLower()) ||
                roleClaim.Value.ToLower().Equals(PolicyConstants.Admin.ToLower()))
            {
                return true;
            }
        }
        
        return false;
    }
    
}