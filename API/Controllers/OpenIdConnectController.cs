using System;
using System.Threading.Tasks;
using API.Constants;
using API.Data;
using API.DTOs;
using API.Entities.Enums;
using API.Services;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[Authorize(PolicyConstants.Admin)]
public class OpenIdConnectController(ILogger<SettingsController> logger, IUnitOfWork unitOfWork,
    ILocalizationService localizationService): BaseApiController
{
    [AllowAnonymous]
    [HttpGet("info")]
    public async Task<ActionResult<OpenIdConnectInfoDto>> GetOpenIdConnectInfo()
    {
        var settingDto = await unitOfWork.SettingsRepository.GetSettingsDtoAsync();
        if (string.IsNullOrEmpty(settingDto.OpenIdAuthority))
        {
            return NotFound(await localizationService.Translate("", "open-id-not-set-up"));
        }
        
        return Ok(new OpenIdConnectInfoDto
        {
            Authority = settingDto.OpenIdAuthority,
            Provider = settingDto.OpenIdProvider,
        });
    }

    [AllowAnonymous]
    [HttpGet("is-setup")]
    public async Task<bool> IsSetup()
    {
        return await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync();
    }

    [AllowAnonymous]
    [HttpPost("first-setup")]
    public async Task<ActionResult<OpenIdConnectInfoDto>> InitialSetup(OpenIdConnectInfoDto infoDto)
    {

        if (await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync())
        {
            logger.LogDebug("Open-ID-Connect was already setup");
            // No explanation is on purpose
            return BadRequest();
        }
        
        return await UpdateOpenIdSettings(infoDto);
    }

    [Authorize(PolicyConstants.Admin)]
    [HttpPost("update")]
    public async Task<ActionResult<OpenIdConnectInfoDto>> UpdateOpenIdSettings(OpenIdConnectInfoDto infoDto, [FromQuery] bool shutdown = false)
    {
        if (!await TestOpenIdConfiguration(infoDto))
        {
            return BadRequest(await localizationService.Translate("invalid-openid-authority", infoDto.Authority));
        }
        
        var settings = await unitOfWork.SettingsRepository.GetSettingsAsync();

        foreach (var setting in settings)
        {
            switch (setting.Key)
            {
                case ServerSettingKey.OpenIdAuthority:
                    setting.Value = infoDto.Authority;
                    break;
                case ServerSettingKey.OpenIdConnectProviders:
                    setting.Value = infoDto.Provider.ToString();
                    break;
            }
        }

        try
        {
            if (unitOfWork.HasChanges())
            {
                await unitOfWork.CommitAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update OpenId Connect settings");
            return StatusCode(500);
        }
        
        return Ok(infoDto);
    }
    
    private async Task<bool> TestOpenIdConfiguration(OpenIdConnectInfoDto dto)
    {
        if (string.IsNullOrEmpty(dto.Authority))
        {
            return false;
        }

        var url = dto.Authority + "/.well-known/openid-configuration";
        try
        {
            var resp = await url.GetAsync();
            return resp.StatusCode == 200;
        }
        catch (Exception e)
        {
            logger.LogError("OpenIdConfiguration failed: {Reason}", e.Message);
            return false;
        }
    }
}