using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities.Enums;
using API.Extensions;
using API.Services;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class SettingsController(ILogger<SettingsController> logger, IUnitOfWork unitOfWork,
    ILocalizationService localizationService): BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<ServerSettingDto>> GetSettings()
    {
        var settingDto = await unitOfWork.SettingsRepository.GetSettingsDtoAsync();
        return Ok(settingDto);
    }

    [AllowAnonymous]
    [HttpPost("update-settings-first")]
    public async Task<ActionResult<ServerSettingDto>> UpdateSettingsFirst(ServerSettingDto settingDto)
    {
        if (await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync())
        {
            return BadRequest(await localizationService.Translate("", "settings-not-first"));
        }

        return await UpdateSettings(settingDto);
    }

    [HttpPost("update-settings")]
    public async Task<ActionResult<ServerSettingDto>> UpdateSettings(ServerSettingDto settingDto)
    {
        logger.LogInformation("{UserName} is updating server settings", User.GetName());

        if (!await TestOpenIDConfiguration(settingDto))
        {
            return BadRequest(
                await localizationService.Translate("", "invalid-openid-authority", settingDto.OpenIdAuthority ?? "<empty>"));
        }
        
        
        var currentSettings = await unitOfWork.SettingsRepository.GetSettingsAsync();

        foreach (var setting in currentSettings)
        {
            switch (setting.Key)
            {
                case ServerSettingKey.LoggingLevel:
                    if (setting.Value != settingDto.LoggingLevel)
                    {
                        setting.Value = settingDto.LoggingLevel;
                    }
                    break;
                case ServerSettingKey.OpenIdAuthority:
                    if (setting.Value != settingDto.OpenIdAuthority 
                        && !string.IsNullOrEmpty(settingDto.OpenIdAuthority))
                    {
                        setting.Value = settingDto.OpenIdAuthority;
                    }
                    break;
                case ServerSettingKey.OpenIdClientId:
                    if (setting.Value != settingDto.OpenIdClientId 
                        && !string.IsNullOrEmpty(settingDto.OpenIdClientId))
                    {
                        setting.Value = settingDto.OpenIdClientId;
                    }
                    break;
                case ServerSettingKey.OpenIdClientSecret:
                    if (setting.Value != settingDto.OpenIdClientSecret 
                        && !string.IsNullOrEmpty(settingDto.OpenIdClientSecret))
                    {
                        setting.Value = settingDto.OpenIdClientSecret;
                    }
                    break;
            }
        }

        // TODO: Fail if OpenId Connect fields are not all set
        

        if (!unitOfWork.HasChanges())
        {
            return Ok(settingDto);
        }

        try
        {
            await unitOfWork.CommitAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred while updating server settings");
            await unitOfWork.RollbackAsync();
            return BadRequest("An exception occurred while updating server settings");
        }
        
        logger.LogInformation("Successfully updated server settings");
        return Ok(settingDto);
    }

    private async Task<bool> TestOpenIDConfiguration(ServerSettingDto dto)
    {
        if (string.IsNullOrEmpty(dto.OpenIdAuthority))
        {
            return false;
        }

        var url = dto.OpenIdAuthority + ".well-known/openid-configuration";
        try
        {
            var resp = await url.GetAsync();
            return resp.StatusCode == 200;
        }
        catch (Exception)
        {
            /* Swallow exception */
            return false;
        }
    }
    
}