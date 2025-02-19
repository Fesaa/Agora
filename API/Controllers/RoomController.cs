using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class RoomController(ILogger<RoomController> logger, IRoomService roomService, IMapper mapper,
    IUnitOfWork unitOfWork): BaseApiController
{

    [HttpGet("all-rooms")]
    public async Task<List<MeetingRoomDto>> All()
    {
        return await unitOfWork.RoomRepository.GetMeetingRooms();
    }


    [HttpPost("create")]
    public async Task<ActionResult<MeetingRoomDto>> Create(MeetingRoomDto meetingRoomDto)
    {
        var room = await roomService.Create(meetingRoomDto);
        return Ok(mapper.Map<MeetingRoomDto>(room));
    }
    
}