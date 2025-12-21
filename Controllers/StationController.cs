using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Stations;
using TankR.Data.Models;
using TankR.Repos.Implementations;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;


[ApiController]
[Route("[controller]")]
public class StationController: ControllerBase
{
    private readonly IStationRepo _stationRepo;
    private readonly IMapper _mapper;

    
    public StationController(IStationRepo stationRepo, IMapper mapper)
    {
        _stationRepo = stationRepo;
        _mapper = mapper;
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

    [HttpPost]
    public async Task<ActionResult> Add(CreateStationDto createStationDto)
    {
        try
        {
            var station = _mapper.Map<Station>(createStationDto);
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

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateStationDto updateStationDto)
    {
        try
        {
            var station = await _stationRepo.GetById(id);

            if (station == null)
                return NotFound();
            _mapper.Map(updateStationDto, station);
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