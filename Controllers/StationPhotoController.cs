using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.StationFuelPrices;
using TankR.Data.Dtos.StationPhotos;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[ApiController]
[Route("[controller]")]
public class StationPhotoController: ControllerBase
{
    private readonly IStationPhotoRepo _stationPhotoRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IMapper _mapper;

    public StationPhotoController(IStationPhotoRepo stationPhotoRepo, IStationRepo stationRepo, IMapper mapper)
    {
        _stationPhotoRepo = stationPhotoRepo;
        _stationRepo = stationRepo;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationPhotoDto>?>> GetAll()
    {
        try
        {
            var stationPhotos = await _stationPhotoRepo.GetAll();
            var result = _mapper.Map<IEnumerable<StationPhotoDto>>(stationPhotos);
            return Ok(result);
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [HttpGet("{stationId:int}")]
    public async Task<ActionResult<IEnumerable<StationPhotoDto>?>> GetAllByStationId(int stationId)
    {
        try
        {
            var station = await _stationRepo.GetById(stationId);
            if(station == null)
                return NotFound($"Station with ID {stationId} not found");
            
            var stationPhotos = await _stationPhotoRepo.GetAllByStationId(stationId);
            var result = _mapper.Map<IEnumerable<StationPhotoDto>>(stationPhotos);
            return Ok(result);
        }catch(Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [HttpGet("photo/{id:int}")]
    public async Task<ActionResult<StationPhotoDto>> GetById(int id)
    {
        var photo = await _stationPhotoRepo.GetPhotoById(id);
        if (photo == null)
            return NotFound();

        return Ok(_mapper.Map<StationPhotoDto>(photo));
    }

    [HttpPost]
    public async Task<IActionResult> SavePhoto(CreateStationPhotoDto photoDto)
    {
        try
        {
            var station = await _stationRepo.GetById(photoDto.StationId);
            if (station == null)
                return NotFound($"Station with ID {photoDto.StationId} not found");

            var photo = _mapper.Map<StationPhoto>(photoDto);
            await _stationPhotoRepo.SavePhoto(photo);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = photo.Id },
                photoDto
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
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remove(int id)
    {
        try
        {
            var photo = await _stationPhotoRepo.GetPhotoById(id);
            if (photo == null)
                return NotFound($"Photo with ID {id} not found");

            
            await _stationPhotoRepo.DeletePhoto(photo);
            return Ok(new { message = "Photo removed successfully" });
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
    
}