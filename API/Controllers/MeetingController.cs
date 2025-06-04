using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Constants;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Exceptions;
using API.Extensions;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class MeetingController(ILogger<MeetingController> logger, IMeetingService meetingService,
    IUnitOfWork unitOfWork, IRoomService roomService, ILocalizationService localizationService,
    IMapper mapper): BaseApiController
{

    [HttpPost]
    public async Task<ActionResult> CreateMeeting(MeetingDto meetingDto)
    {
        var userId = User.GetIdentifier();
        if (meetingDto.CreatorId != "")
        {
            if (!User.HasPolicyClaimOrAdmin(PolicyConstants.CreateForOther)) return Unauthorized();
            
            var creatorId = await unitOfWork.EmailsRepository.GetIdByEmail(meetingDto.CreatorId);
            userId = creatorId ?? throw new AgoraException("meeting-creator-id");
        }
        
        try
        {
            await meetingService.CreateMeeting(userId, meetingDto);
        }
        catch (AgoraException agoraException)
        {
            return BadRequest(await localizationService.Translate(agoraException.Message));
        }
        return Ok();
    }

    [HttpPost("update")]
    [Authorize(PolicyConstants.Admin)]
    public async Task<ActionResult> UpdateMeeting(MeetingDto meetingDto)
    {
        await meetingService.UpdateMeeting(meetingDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMeeting(int id)
    {
        await meetingService.DeleteMeeting(id);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<MeetingDto>>> GetTodaysMeetings([FromQuery] bool userOnly = false, [FromQuery] int? roomId = null)
    {
        if (!userOnly && !User.HasPolicyClaim(PolicyConstants.Admin))
        {
            throw new UnauthorizedAccessException();
        }
        
        var now = DateTime.UtcNow;
        var midnightUtc = now.Date.AddDays(1).AddTicks(-1);
        var startUtc = midnightUtc.AddDays(-1);

        List<MeetingQueryOption> opts = [
            MeetingRepository.EndAfter(now),
            MeetingRepository.EndBefore(midnightUtc),
            MeetingRepository.StartAfter(startUtc)
        ];

        if (userOnly)
        {
            opts.Add(MeetingRepository.IsAttending(User.GetIdentifier()));
        }

        if (roomId != null)
        {
            opts.Add(MeetingRepository.InRoom(roomId.Value));
        }
        
        var meetings = await unitOfWork.MeetingRepository.GetMeetingDtos([.. opts]);
        return Ok(meetings);
    }

    // TODO: pagination?
    [AllowAnonymous]
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MeetingDto>>> GetUpcomingMeetings([FromQuery] bool userOnly = false, [FromQuery] int dayOffSet = 0)
    {
        var start = DateTime.UtcNow.Date;
        if (dayOffSet > 0)
        {
            start = start.Date.AddDays(dayOffSet);
        }
        
        List<MeetingQueryOption> opts = [
            MeetingRepository.StartAfter(start),
            MeetingRepository.WithRoom(),
        ];

        if (userOnly)
        {
            opts.Add(MeetingRepository.IsAttending(User.GetIdentifier()));
        }

        var meetings = await unitOfWork.MeetingRepository.GetMeetingDtos([.. opts]);
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
    {
        var meeting = await unitOfWork.MeetingRepository.GetMeetingById(id, MeetingIncludes.Room | MeetingIncludes.Facilities);
        if (meeting == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<MeetingDto>(meeting));
    }

    [HttpGet("attendees")]
    public async Task<ActionResult<IEnumerable<UserEmailDto>>> GetAttendees([FromQuery] string mustContain)
    {
        var emails = await unitOfWork.EmailsRepository
            .GetEmailsAsync(EmailsRepository.Contains(mustContain));
        return Ok(emails);
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<IEnumerable<MeetingRoomDto>>> GetRooms(RoomsOnDto dto)
    {
        if (dto.Start == null || dto.End == null)
        {
            return Ok(await unitOfWork.RoomRepository.GetMeetingRooms());
        }

        return Ok(await roomService.AvailableRoomsOn(dto.Start.Value, dto.End.Value));
    }

    [HttpPost("ack")]
    [Authorize(PolicyConstants.Admin)]
    public async Task<IActionResult> AckMeeting([FromQuery] int meetingId, [FromQuery] bool ack)
    {
        await meetingService.AcknowledgeMeeting(meetingId, ack);
        return Ok();
    }

    [HttpGet("require-ack")]
    [Authorize(PolicyConstants.Admin)]
    public async Task<ActionResult<IList<MeetingDto>>> RequireAck()
    {
        var meetings = await unitOfWork.MeetingRepository.GetMeetings(
            MeetingRepository.StartAfter(DateTime.Now), 
            MeetingRepository.NeedsAck()
            );
        
        return Ok(meetings);
    }
    
}