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