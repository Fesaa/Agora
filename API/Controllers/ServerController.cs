using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ServerController(IUnitOfWork unitOfWork): BaseApiController
{

    [AllowAnonymous]
    [HttpGet("is-first-startup")]
    public async Task<bool> IsFirstStartup()
    {
        return !await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync();
    }
    
}