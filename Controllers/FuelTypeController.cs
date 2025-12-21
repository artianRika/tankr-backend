using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.FuelTypes;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;


[ApiController]
[Route("[controller]")]
public class FuelTypeController : ControllerBase
{
    private readonly IFuelTypeRepo _fuelTypeRepo;
    private readonly IMapper _mapper;

    public FuelTypeController(IFuelTypeRepo fuelTypeRepo, IMapper mapper)
    {
        _fuelTypeRepo = fuelTypeRepo;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuelTypeDto>>> GetAll()
    {
        try
        {
            var fuelTypes = await _fuelTypeRepo.GetAll();

            var result = _mapper.Map<IEnumerable<FuelTypeDto>>(fuelTypes);
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
    public async Task<ActionResult<FuelType>> GetById(int id)
    {
        try
        {
            var fuelType = await _fuelTypeRepo.GetById(id);
            if (fuelType == null)
                return NotFound($"FuelType with id {id} not found");
            var result = _mapper.Map<FuelTypeDto>(fuelType);

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
    
    [HttpGet("{name}")]
    public async Task<ActionResult<FuelType>> GetByName(string name)
    {
        try
        {
            var fuelType = await _fuelTypeRepo.GetByName(name);
            if (fuelType == null)
                return NotFound($"FuelType with name {name} not found");
            var result = _mapper.Map<FuelTypeDto>(fuelType);

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
    public async Task<ActionResult> Add(CreateFuelTypeDto createFuelTypeDto)
    {
        try
        {
            var fuelType = _mapper.Map<FuelType>(createFuelTypeDto);
            await _fuelTypeRepo.Add(fuelType);


            return CreatedAtAction(
                nameof(GetById),
                new { id = fuelType.Id },
                _mapper.Map<FuelTypeDto>(fuelType)
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
    public async Task<ActionResult> Update(int id, UpdateFuelTypeDto updateFuelTypeDto)
    {
        try
        {
            var fuelType = await _fuelTypeRepo.GetById(id);

            if (fuelType == null)
                return NotFound();
            _mapper.Map(updateFuelTypeDto, fuelType);
            await _fuelTypeRepo.Update(fuelType);

            return Ok(_mapper.Map<FuelTypeDto>(fuelType));
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