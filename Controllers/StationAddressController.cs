using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.StationAddresses;
using TankR.Data.Dtos.Stations;
using TankR.Data.Dtos.UserAddresses;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StationAddressController: ControllerBase
{
    private readonly IStationAddressRepo _stationAddressRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IMapper _mapper;

    public StationAddressController(IStationAddressRepo stationAddressRepo, IStationRepo stationRepo, IMapper mapper)
    {
        _stationAddressRepo = stationAddressRepo;
        _stationRepo = stationRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationAddressDto>>> GetAll()
    {
        try
        {
            var addresses = await _stationAddressRepo.GetAll();
            var result = _mapper.Map<IEnumerable<StationAddressDto>>(addresses);
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
    public async Task<ActionResult<StationAddressDto>> GetById(int id)
    {
        try
        {
            var address = await _stationAddressRepo.GetById(id);
            if (address == null)
                return NotFound($"Address with id {id} not found");
            var result = _mapper.Map<StationAddressDto>(address);
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
    public async Task<ActionResult> Add(CreateStationAddressDto createStationAddressDto)
    {
        try
        {
            var address = _mapper.Map<StationAddress>(createStationAddressDto);
            await _stationAddressRepo.Add(address);


            var station = await _stationRepo.GetById(createStationAddressDto.StationId);
            if (station == null) 
                return NotFound($"Station with ID {createStationAddressDto.StationId} not found");
            

            var stationDto = _mapper.Map<StationDto>(station);
            return CreatedAtAction(nameof(_stationRepo.GetById), new { id = station.Id }, stationDto);
            
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
    public async Task<ActionResult> Update(int id, UpdateStationAddressDto updateStationAddressDto)
    {
        try
        {
            var address = await _stationAddressRepo.GetById(id);
            if (address == null)
                return NotFound();

            _mapper.Map(updateStationAddressDto, address);
            await _stationAddressRepo.Update(address);

            return Ok(_mapper.Map<StationAddressDto>(address));
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