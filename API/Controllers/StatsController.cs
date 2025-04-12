using API.Data;
using API.Services;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class StatsController(ILogger<StatsController> logger, IMeetingService meetingService,
    IUnitOfWork unitOfWork, IRoomService roomService, ILocalizationService localizationService): BaseApiController
{
    
    
}