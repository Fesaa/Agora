using System;
using System.Threading.Tasks;
using API.Data.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Data;

public interface IUnitOfWork
{
    
    ISettingsRepository SettingsRepository { get; }
    UserPreferencesRepository UserPreferencesRepository { get; }
    
    bool Commit();
    Task<bool> CommitAsync();
    bool HasChanges();
    Task<bool> RollbackAsync();
}

public class UnitOfWork(DataContext context, IMapper mapper, ILogger<UnitOfWork> logger) : IUnitOfWork
{

    public ISettingsRepository SettingsRepository { get; } = new SettingsRepository(context, mapper);
    public UserPreferencesRepository UserPreferencesRepository { get; } = new UserPreferencesRepository(context, mapper);

    public bool Commit()
    {
        return context.SaveChanges() > 0;
    }

    public async Task<bool> CommitAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }

    public async Task<bool> RollbackAsync()
    {
        try
        {
            await context.Database.RollbackTransactionAsync();
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while rolling back changed {Error}", e);
            return false;
        }

        return true;
    }
}