using System.Threading.Tasks;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class AccountController(ILogger<AccountController> logger, ILocalizationService localizationService): BaseApiController
{

    [HttpGet("name")]
    public string Name()
    {
        return User.GetName() ?? "No name";
    }

    [Authorize("admin")]
    [HttpGet("test")]
    public async Task<string> Test()
    {
        return await localizationService.Translate(User.GetIdentifier(), "secret");
    }
}