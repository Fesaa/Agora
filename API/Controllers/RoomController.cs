using System.Collections.Generic;
using System.Threading.Tasks;
using API.Constants;
using API.Data;
using API.DTOs;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("update")]
    public async Task<ActionResult<MeetingRoomDto>> Update(MeetingRoomDto meetingRoomDto)
    {
        var room = await roomService.Update(meetingRoomDto);
        return Ok(mapper.Map<MeetingRoomDto>(room));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, bool force = false)
    {
        force &= User.IsInRole(PolicyConstants.AdminRole);
        await roomService.Delete(id, force);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingRoomDto>> GetMeetingRoom(int id)
    {
        var room = await unitOfWork.RoomRepository.GetMeetingRoom(id);
        if (room == null) return NotFound();
        return Ok(mapper.Map<MeetingRoomDto>(room));
    }
    
}