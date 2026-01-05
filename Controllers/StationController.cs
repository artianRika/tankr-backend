using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Stations;
using TankR.Data.Models;
using TankR.Repos.Implementations;
using TankR.Repos.Interfaces;
using TankR.Services.Interfaces;

namespace TankR.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class StationController: ControllerBase
{
    private readonly IStationRepo _stationRepo;
    private readonly IMapper _mapper;
    private readonly IFreeImageService _freeImageService;


    
    public StationController(IStationRepo stationRepo, IMapper mapper,  IFreeImageService freeImageService)
    {
        _stationRepo = stationRepo;
        _mapper = mapper;
        _freeImageService = freeImageService;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDto>>> GetAll()
    {
        try
        {
            var stations = await _stationRepo.GetAll();

            var result = _mapper.Map<IEnumerable<StationDto>>(stations);
            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StationDto>> GetById(int id)
    {
        try
        {
            var station = await _stationRepo.GetById(id);
            if (station == null)
                return NotFound($"station with id {id} not found");
            var result = _mapper.Map<StationDto>(station);

            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> Add([FromForm] CreateStationDto createStationDto)
    {
        try
        {
            var station = _mapper.Map<Station>(createStationDto);
            
            
            if (createStationDto.Logo != null)
            {
                var logoUrl =
                    await _freeImageService.UploadAsync(createStationDto.Logo);

                station.LogoUrl = logoUrl;
            }
            
            station.CreatedAt = DateTime.UtcNow;

            
            await _stationRepo.Add(station);
            

            return CreatedAtAction(
                nameof(GetById),
                new { id = station.Id },
                _mapper.Map<StationDto>(station)
            );
        }
        catch (Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromForm] UpdateStationDto updateStationDto)
    {
        try
        {
            var station = await _stationRepo.GetById(id);

            if (station == null)
                return NotFound();
            
            _mapper.Map(updateStationDto, station);
            
            
            
            if (updateStationDto.Logo != null)
            {
                var logoUrl =
                    await _freeImageService.UploadAsync(updateStationDto.Logo);

                station.LogoUrl = logoUrl;
            }
            station.UpdatedAt = DateTime.UtcNow;

            await _stationRepo.Update(station);

            return Ok(_mapper.Map<StationDto>(station));
        }
        catch (Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}