using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Address;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressController: ControllerBase
{
    private readonly IAddressRepo _addressRepo;
    private readonly IUserRepo _userRepo;
    private readonly IMapper _mapper;

    public AddressController(IAddressRepo addressRepo, IUserRepo userRepo, IMapper mapper)
    {
        _addressRepo = addressRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

      [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
    {
        try
        {
            var addresses = await _addressRepo.GetAll();
            var result = _mapper.Map<IEnumerable<AddressDto>>(addresses);
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
    public async Task<ActionResult<AddressDto>> GetById(int id)
    {
        try
        {
            var address = await _addressRepo.GetById(id);
            if (address == null)
                return NotFound($"Address with id {id} not found");
            var result = _mapper.Map<AddressDto>(address);
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
    public async Task<ActionResult> Add(CreateAddressDto createAddressDto)
    {
        try
        {
            var address = _mapper.Map<Address>(createAddressDto);
            await _addressRepo.Add(address);


            var user = await _userRepo.GetById(createAddressDto.UserId);
            if (user == null) 
                return NotFound($"User with ID {createAddressDto.UserId} not found");

            user.AddressId = address.Id;
            user.Address = address;
            await _userRepo.Update(user);

            user.Address = address;

            var userDto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(_userRepo.GetById), new { id = user.Id }, userDto);
            
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
    public async Task<ActionResult> Update(int id, UpdateAddressDto updateAddressDto)
    {
        try
        {
            var address = await _addressRepo.GetById(id);
            if (address == null)
                return NotFound();

            _mapper.Map(updateAddressDto, address);
            await _addressRepo.Update(address);

            return Ok(_mapper.Map<AddressDto>(address));
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