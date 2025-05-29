using System.Collections.Generic;
using System.Threading.Tasks;
using API.Constants;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

public class FacilitiesController(ILogger<FacilitiesController> logger, IUnitOfWork unitOfWork,
    IFacilityService facilityService): BaseApiController
{

    [HttpGet("all")]
    public async Task<ActionResult<List<FaclitiyDto>>> GetAll([FromQuery] bool activeOnly)
    {
        return Ok(await unitOfWork.FacilityRepository.AllDtos(activeOnly));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FaclitiyDto>> Get(int id)
    {
        return Ok(await unitOfWork.FacilityRepository.GetById(id));
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<List<FaclitiyDto>>> GetForRoom(int roomId)
    {
        return Ok(await unitOfWork.FacilityRepository.GetByRoom(roomId, true));
    }

    [HttpPost("create")]
    public async Task<ActionResult<FaclitiyDto>> Create(FaclitiyDto dto)
    {
        logger.LogDebug("{UserName} is creating a new facility {Name}", User.GetName(), dto.DisplayName);
        var f = await facilityService.CreateAsync(dto);
        return Ok(f);
    }

    [HttpPost("update")]
    public async Task<ActionResult<FaclitiyDto>> Update(FaclitiyDto dto)
    {
        logger.LogDebug("{UserName} is trying to updating facility {Id}", User.GetName(), dto.Id);
        var f = await facilityService.UpdateAsync(dto);
        return Ok(f);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<FaclitiyDto>> Delete(int id, [FromQuery] bool force = false)
    {
        force &= User.IsInRole(PolicyConstants.Admin);

        logger.LogDebug("{UserName} is trying to deleting facility {Id}", User.GetName(), id);

        await facilityService.DeleteAsync(id, force);
        return Ok();
    }

    [HttpPost("activate/{id}")]
    public async Task<IActionResult> Activate(int id)
    {
        await facilityService.ActiveAsync(id);
        return Ok();
    }

    [HttpPost("deactivate/{id}")]
    public async Task<ActionResult> Deactivate(int id)
    {
        await facilityService.DeActivateAsync(id);
        return Ok();
    }
    
}