using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services;

public interface INotifyService
{
    Task Notify(IList<string> userIds);
}

public class NotifyService: INotifyService
{

    public async Task Notify(IList<string> userIds)
    {
        
    }
}