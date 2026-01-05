using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.StationEmployees;
using TankR.Data.Enums;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StationEmployeeController : ControllerBase
{
    private readonly IStationEmployeeRepo _stationEmployeeRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IUserRepo _userRepo;
    private readonly IMapper _mapper;

    public StationEmployeeController(IStationEmployeeRepo stationEmployeeRepo, IStationRepo stationRepo, IUserRepo userRepo, IMapper mapper)
    {
        _stationEmployeeRepo = stationEmployeeRepo;
        _stationRepo = stationRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("station/{stationId:int}")]
    public async Task<ActionResult<IEnumerable<StationEmployeeDto>>> GetEmployeesByStation(int stationId)
    {
        try
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null)
                return NotFound($"Station with ID {stationId} not found");
            
            var employees = await _stationEmployeeRepo.GetEmployeesByStation(stationId);
            var dtos = _mapper.Map<IEnumerable<StationEmployeeDto>>(employees);
            return Ok(dtos);
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<ActionResult> Assign(AssignEmployeeDto dto)
    {
        try
        {
            
            var station = await _stationRepo.GetById(dto.StationId);
            if (station == null)
                return NotFound($"Station with ID {dto.StationId} not found");

            var user = await _userRepo.GetById(dto.UserId);
            if (user == null)
                return NotFound($"User with ID {dto.UserId} not found");
            
            if (user.Role != UserRole.Cashier)
                return BadRequest("Only users with Cashier role can be assigned to a station");


            var exists = await _stationEmployeeRepo.Exists(dto.StationId, dto.UserId);
            if (exists)
                return Conflict($"User {dto.UserId} is already assigned to station {dto.StationId}");

            var stationEmployee =  _mapper.Map<StationEmployee>(dto);
            await _stationEmployeeRepo.Assign(stationEmployee);
            
            return Ok(new { message = "Employee assigned successfully" });
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("remove")]
    public async Task<ActionResult> Remove([FromBody] AssignEmployeeDto dto)
    {
        try
        {
            
            var station = await _stationRepo.GetById(dto.StationId);
            if (station == null)
                return NotFound($"Station with ID {dto.StationId} not found");

            var user = await _userRepo.GetById(dto.UserId);
            if (user == null)
                return NotFound($"User with ID {dto.UserId} not found");

            var exists = await _stationEmployeeRepo.Exists(dto.StationId, dto.UserId);
            if (!exists)
                return NotFound($"User {dto.UserId} is not assigned to station {dto.StationId}");

            await _stationEmployeeRepo.Remove(dto.StationId, dto.UserId);
            return Ok(new { message = "Employee removed successfully" });
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("exists")]
    public async Task<ActionResult> Exists([FromQuery] int stationId, [FromQuery] int userId)
    {
        try
        {
            var exists = await _stationEmployeeRepo.Exists(stationId, userId);
            return Ok(new { exists });
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}