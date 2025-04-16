using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task NotifyAttendeesAsync(IEnumerable<string> attendeeIds, string message)
    {
        // log the attendees and the message (for now)
        foreach (var attendeeId in attendeeIds)
        {
            _logger.LogInformation($"Notifying attendee {attendeeId}: {message}");
        }

        
        await Task.CompletedTask;
    }
}
