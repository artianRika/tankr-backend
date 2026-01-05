using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.StationFuelPrices;
using TankR.Data.Models;
using TankR.Repos.Implementations;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[Authorize]
[ApiController]
[Route("api/station-fuel-price/")]
public class StationFuelPriceController: ControllerBase
{
    private readonly IStationFuelPriceRepo _stationFuelPriceRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IFuelTypeRepo _fuelTypeRepo;
    private readonly IMapper _mapper;

    public StationFuelPriceController(IStationFuelPriceRepo stationFuelPriceRepo, IStationRepo stationRepo, IFuelTypeRepo fuelTypeRepo, IMapper mapper)
    {
        _stationFuelPriceRepo = stationFuelPriceRepo;
        _stationRepo = stationRepo;
        _fuelTypeRepo = fuelTypeRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationFuelPriceDto>?>> GetAll()
    {
        try
        {
            var fuelPrices = await _stationFuelPriceRepo.GetAll();
            var result = _mapper.Map<IEnumerable<StationFuelPriceDto>>(fuelPrices);
            return Ok(result);
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpGet("station/{stationId:int}")]
    public async Task<ActionResult<IEnumerable<StationFuelPriceDto>?>> GetAllByStationId(int stationId)
    {
        try
        {
            var station = await _stationRepo.GetById(stationId);
            if(station == null)
                return NotFound($"Station with ID {stationId} not found");
            
            var fuelPrices = await _stationFuelPriceRepo.GetAllByStation(stationId);
            var result = _mapper.Map<IEnumerable<StationFuelPriceDto>>(fuelPrices);
            return Ok(result);
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [HttpGet("station/{stationId:int}/fueltype/{fuelTypeId:int}")]
    public async Task<ActionResult<StationFuelPriceDto>?> Get(int stationId, int fuelTypeId)
    {
        try
        {
            var fuelPrice = await _stationFuelPriceRepo.Get(stationId, fuelTypeId);
            var result = _mapper.Map<StationFuelPriceDto>(fuelPrice);
            return Ok(new{ price = result.Price});
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("station/{stationId:int}/fueltype/{fuelTypeId:int}")]
    public async Task<ActionResult<StationFuelPriceDto>> SetPrice(int stationId, int fuelTypeId, decimal price)
    {
        try
        {
            var station = await _stationRepo.GetById(stationId);
            var fuelType = await _fuelTypeRepo.GetById(fuelTypeId);
            
            if(station == null)
                return NotFound($"Station with ID {stationId} not found");
            if(fuelType == null)
                return NotFound($"FuelType with ID {fuelTypeId} not found");
            
            await _stationFuelPriceRepo.SetPrice(stationId, fuelTypeId, price);

            var savedEntity = await _stationFuelPriceRepo.Get(stationId, fuelTypeId);
            if (savedEntity == null)
                return NotFound($"Price for Station {stationId} and FuelType {fuelTypeId} not found after saving");

            var resultDto = _mapper.Map<StationFuelPriceDto>(savedEntity);

            return CreatedAtAction(
                nameof(Get),  
                new { stationId = resultDto.StationId, fuelTypeId = resultDto.FuelTypeId },
                resultDto
            );
            
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("station/{stationId:int}/fueltype/{fuelTypeId:int}")]
    public async Task<ActionResult<StationFuelPriceDto>> UpdatePrice(int stationId, int fuelTypeId, decimal newPrice)
    {
        try
        {
            var station = await _stationRepo.GetById(stationId);
            var fuelType = await _fuelTypeRepo.GetById(fuelTypeId);

            if(station == null)
                return NotFound($"Station with ID {stationId} not found");
            if(fuelType == null)
                return NotFound($"FuelType with ID {fuelTypeId} not found");

            await _stationFuelPriceRepo.UpdatePrice(stationId, fuelTypeId, newPrice);

            var updatedEntity = await _stationFuelPriceRepo.Get(stationId, fuelTypeId);
            if (updatedEntity == null)
                return NotFound($"Price for Station {stationId} and FuelType {fuelTypeId} not found after update");

            var resultDto = _mapper.Map<StationFuelPriceDto>(updatedEntity);

            return Ok(resultDto);

        } 
        catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}