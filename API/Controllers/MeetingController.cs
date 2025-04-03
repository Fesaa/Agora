using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class MeetingController(ILogger<MeetingController> logger, IMeetingService meetingService,
    IUnitOfWork unitOfWork, IRoomService roomService): BaseApiController
{

    [HttpPost]
    public async Task<ActionResult> CreateMeeting(MeetingDto meetingDto)
    {
        await meetingService.CreateMeeting(User.GetIdentifier(), meetingDto);
        return Ok();
    }

    [HttpPost("update")]
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

    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<MeetingDto>>> GetTodaysMeetings()
    {
        var now = DateTime.UtcNow;
        var midnightUtc = now.Date.AddDays(1).AddTicks(-1);
        var startUtc = midnightUtc.AddDays(-1);
        
        var meetings = await unitOfWork.MeetingRepository.GetMeetingDtos(
            MeetingRepository.EndAfter(now),
            MeetingRepository.EndBefore(midnightUtc),
            MeetingRepository.StartAfter(startUtc));
        return Ok(meetings);
    }

    // TODO: pagination?
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MeetingDto>>> GetUpcomingMeetings()
    {
        var meetings = await unitOfWork.MeetingRepository.GetMeetingDtos(
            MeetingRepository.StartAfter(DateTime.UtcNow));
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
    {
        var meeting = await unitOfWork.MeetingRepository.GetMeetingById(id);
        if (meeting == null)
        {
            return NotFound();
        }
        return Ok(meeting);
    }

    [HttpGet("attendees")]
    public async Task<ActionResult<IEnumerable<UserEmailDto>>> GetAttendees([FromQuery] string mustContain)
    {
        var emails = await unitOfWork.EmailsRepository
            .GetEmailsAsync(EmailsRepository.Contains(mustContain));
        return Ok(emails);
    }

    [HttpGet("slots/{meetingId}")]
    public async Task<ActionResult<IEnumerable<MeetingSlot>>> GetSlots(int meetingId, [FromQuery] long unixTime)
    {
        if (unixTime == null)
        {
            return BadRequest();
        }

        var date = unixTime.ToDateTimeFromUnix(); // incorrect
        // TZ info gets lost, and we're back a day. 
        return Ok(await meetingService.AvailableSlotsForOn(meetingId, date));
    }

    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<MeetingRoomDto>>> GetRooms([FromQuery] long? startTime, [FromQuery] long? endTime)
    {
        if (startTime == null || endTime == null)
        {
            return Ok(await unitOfWork.RoomRepository.GetMeetingRooms());
        }
        
        var startDate = startTime.Value.ToDateTimeFromUnix();
        var endDate = endTime.Value.ToDateTimeFromUnix();
        return Ok(await roomService.AvailableRoomsOn(startDate, endDate));
    }
    
}