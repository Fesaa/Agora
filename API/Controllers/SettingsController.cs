using System;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities.Enums;
using API.Extensions;
using API.Services;
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

    /// <remarks>Does not update OpenIdConnect settings</remarks>
    [HttpPost("update-settings")]
    public async Task<ActionResult<ServerSettingDto>> UpdateSettings(ServerSettingDto settingDto)
    {
        logger.LogInformation("{UserName} is updating server settings", User.GetName());
        
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
            }
        }

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
    
}