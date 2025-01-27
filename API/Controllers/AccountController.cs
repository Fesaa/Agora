using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController: BaseApiController
{

    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}