namespace API.Services;

public interface INotificationService
{
    Task NotifyAttendeesAsync(IEnumerable<string> attendeeIds, string message);
}
