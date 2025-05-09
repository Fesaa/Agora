using System;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Entities.Enums;
using API.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Services;

public interface IThemeService
{
    /// <summary>
    /// Returns the theme's css as a string
    /// </summary>
    /// <param name="themeId">Theme id</param>
    /// <returns>CSS</returns>
    Task<string> GetThemeContent(int themeId);
    /// <summary>
    /// Delete a theme from the DataContext and disk.
    /// </summary>
    /// <param name="themeId">Theme id</param>
    /// <returns>Wether a theme was deleted</returns>
    Task<bool> DeleteTheme(int themeId);
    /// <summary>
    /// Create a new theme from a file on disk
    /// </summary>
    /// <param name="path">The theme's location on disk</param>
    /// <param name="userName">The user uploading the theme</param>
    /// <returns>The newly created theme</returns>
    Task<Theme> ThemeFromFile(string path, string userName);
    /// <summary>
    /// Updates the default theme
    /// </summary>
    /// <param name="themeId"></param>
    Task SetDefaultTheme(int themeId);
}

public class ThemeService(IUnitOfWork unitOfWork, IDirectoryService directoryService,
    ILogger<ThemeService> logger, DataContext context): IThemeService
{
    public async Task<string> GetThemeContent(int themeId)
    {
        var theme = await unitOfWork.ThemeRepository.GetThemeByIdAsync(themeId);
        if (theme == null)
        {
            throw new AgoraException("theme-doesnt-exist");
        }
        
        var path = directoryService.FileSystem.Path.Join(directoryService.ThemeDirectory, theme.FileName);
        if (string.IsNullOrWhiteSpace(path) || !directoryService.FileSystem.File.Exists(path))
        {
            throw new AgoraException("theme-doesnt-exist");
        }

        return await directoryService.FileSystem.File.ReadAllTextAsync(path);
    }

    public async Task<bool> DeleteTheme(int themeId)
    {
        var theme = await unitOfWork.ThemeRepository.GetThemeByIdAsync(themeId);
        if (theme == null)
        {
            return false;
        }

        if (theme.ThemeProvider == Provider.System)
        {
            throw new AgoraException("cant-delete-system-theme");
        }

        if (theme.Default)
        {
            throw new AgoraException("cant-delete-default-theme");
        }

        logger.LogInformation("Deleting theme {ThemeName}", theme.Name);
        var path = directoryService.FileSystem.Path.Join(directoryService.ThemeDirectory, theme.FileName);
        if (string.IsNullOrWhiteSpace(path) || !directoryService.FileSystem.File.Exists(path))
        {
            logger.LogDebug("File at {Path} does not exist, not removing", path);
            unitOfWork.ThemeRepository.Remove(theme);
            await unitOfWork.CommitAsync();
            return true;
        }

        try
        {
            logger.LogDebug("Deleting file at {Path}", path);
            directoryService.FileSystem.File.Delete(path);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete theme {ThemeName}", theme.Name);
        }

        unitOfWork.ThemeRepository.Remove(theme);
        await unitOfWork.CommitAsync();
        return true;
    }

    public async Task<Theme> ThemeFromFile(string path, string userName)
    {
        if (!directoryService.FileSystem.File.Exists(path))
        {
            logger.LogError("Cannot create new theme as file ({FilePath}) doesn't exist", path);
            throw new AgoraException("themes.errors.file-not-found");
        }
        var themeName = directoryService.FileSystem.Path.GetFileNameWithoutExtension(path);

        if (await unitOfWork.ThemeRepository.GetThemeByNameAsync(themeName) != null)
        {
            throw new AgoraException("themes.errors.in-use");
        }

        directoryService.CopyFileToDirectory(path, directoryService.ThemeDirectory);
        
        var theme = new Theme()
        {
            Name = themeName,
            FileName = directoryService.FileSystem.Path.GetFileName(path),
            ThemeProvider = Provider.User,
            Author = userName,
        };

        unitOfWork.ThemeRepository.Add(theme);
        await unitOfWork.CommitAsync();
        
        logger.LogInformation("Created a new theme {ThemeName} by {UserName}", theme.Name, userName);
        return theme;
    }
    public async Task SetDefaultTheme(int themeId)
    {
        var theme = await unitOfWork.ThemeRepository.GetThemeByIdAsync(themeId);
        if (theme == null)
        {
            throw new AgoraException("theme-doesnt-exist");
        }

        var themes = await context.Themes.ToListAsync();
        themes.ForEach(t => t.Default = false);
        theme.Default = true;
        unitOfWork.ThemeRepository.Update(theme);
        
        await unitOfWork.CommitAsync();
    }
}