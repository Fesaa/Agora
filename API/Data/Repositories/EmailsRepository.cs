using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface IEmailsRepository
{
    Task<IEnumerable<UserEmailDto>> GetEmailsAsync(params EmailsFilter[] filters);
    Task InsertOrUpdate(string userId, string email);
    /// <summary>
    /// Returns the id provided by OIDC
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <remarks>Despite the name, this will also match and return on the external id</remarks>
    Task<string?> GetIdByEmail(string email);
}

public delegate IQueryable<UserEmail> EmailsFilter(IQueryable<UserEmail> emails); 

public class EmailsRepository(DataContext context, IMapper mapper): IEmailsRepository
{

    public async Task<IEnumerable<UserEmailDto>> GetEmailsAsync(params EmailsFilter[] filters)
    {
        var q = context.UserEmails.AsQueryable()
            .AsNoTracking();

        foreach (var filter in filters)
        {
            q = filter(q);
        }
        
        return await mapper.ProjectTo<UserEmailDto>(q).ToListAsync();
    }

    public async Task InsertOrUpdate(string userId, string email)
    {
        var userEmail = await context.UserEmails.Where(ue => ue.ExternalId == userId).FirstOrDefaultAsync();
        if (userEmail == null)
        {
            userEmail = new ()
            {
                ExternalId = userId,
                Email = email,
            };
            context.UserEmails.Add(userEmail);
        }
        else
        {
            userEmail.Email = email;
            context.UserEmails.Update(userEmail);
        }
        
        await context.SaveChangesAsync();
    }
    public async Task<string?> GetIdByEmail(string email)
    {
        return await context.UserEmails
            .Where(ue => ue.Email == email || ue.ExternalId == email)
            .Select(ue => ue.ExternalId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns all emails that contain the passed string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    /// <remarks>If the string is null or empty, every email will be marked as valid by this filter</remarks>
    public static EmailsFilter Contains(string s)
    {
        return emails => emails.Where(e => string.IsNullOrWhiteSpace(s) || e.Email.Contains(s));
    }
}