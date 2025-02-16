using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Exceptions;
using API.Extensions;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class ThemeController(ILogger<ThemeController> logger, IUnitOfWork unitOfWork, IThemeService themeService,
    ILocalizationService localizationService, IDirectoryService directoryService, IMapper mapper)
    : BaseApiController
{

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAllThemes()
    {
        return Ok(await unitOfWork.ThemeRepository.GetThemesAsDtoAsync());
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<string>> GetCss(int themeId)
    {
        try
        {
            return Ok(await themeService.GetThemeContent(themeId));
        }
        catch (AgoraException ex)
        {
            return BadRequest(await localizationService.Get(localizationService.DefaultLocale, ex.Message));
        }
    }

    [HttpPost("set-default")]
    public async Task<IActionResult> SetDefaultTheme(int themeId)
    {
        await themeService.SetDefaultTheme(themeId);
        return Ok();
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ThemeDto>> UploadTheme(IFormFile file)
    {
        if (!file.FileName.EndsWith(".css"))
        {
            return BadRequest();
        }

        if (file.FileName.Contains(".."))
        {
            return BadRequest();
        }
        
        var filePath = await CopyToTemp(file);
        if (filePath == null)
        {
            return BadRequest(await localizationService.Get(localizationService.DefaultLocale, "file-already-exists"));
        }
        
        return Ok(mapper.Map<ThemeDto>(await themeService.ThemeFromFile(filePath, User.GetName())));
        
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTheme(int themeId)
    {
        var found = await themeService.DeleteTheme(themeId);
        return found ? Ok() : NotFound();
    }

    private async Task<string?> CopyToTemp(IFormFile file)
    {
        var path = directoryService.FileSystem.Path.Join(directoryService.TempDirectory, file.FileName);
        if (directoryService.Exists(path))
        {
            logger.LogDebug("Not copying file {FilePath}, file with name already exists", path);
            return null;
        }
        
        await using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);
        return path;
    }
    
}