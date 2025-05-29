using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using API.Constants;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class AccountController(ILogger<AccountController> logger, ILocalizationService localizationService): BaseApiController
{

    [HttpGet("name")]
    public string Name()
    {
        return User.GetName();
    }

    [HttpGet("admin")]
    public bool IsAdmin()
    {
        return User.HasPolicyClaim(PolicyConstants.Admin);
    }

    [HttpGet("roles")]
    public IList<string> Roles()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }
}