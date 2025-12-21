using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.UserAddresses;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[ApiController]
[Route("[controller]")]
public class UserAddressController: ControllerBase
{
    private readonly IUserAddressRepo _userAddressRepo;
    private readonly IUserRepo _userRepo;
    private readonly IMapper _mapper;

    public UserAddressController(IUserAddressRepo userAddressRepo, IUserRepo userRepo, IMapper mapper)
    {
        _userAddressRepo = userAddressRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

      [HttpGet]
    public async Task<ActionResult<IEnumerable<UserAddressDto>>> GetAll()
    {
        try
        {
            var addresses = await _userAddressRepo.GetAll();
            var result = _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
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
    public async Task<ActionResult<UserAddressDto>> GetById(int id)
    {
        try
        {
            var address = await _userAddressRepo.GetById(id);
            if (address == null)
                return NotFound($"Address with id {id} not found");
            var result = _mapper.Map<UserAddressDto>(address);
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
    public async Task<ActionResult> Add(CreateUserAddressDto createUserAddressDto)
    {
        try
        {
            var address = _mapper.Map<UserAddress>(createUserAddressDto);
            await _userAddressRepo.Add(address);


            var user = await _userRepo.GetById(createUserAddressDto.UserId);
            if (user == null) 
                return NotFound($"User with ID {createUserAddressDto.UserId} not found");
            

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
    public async Task<ActionResult> Update(int id, UpdateUserAddressDto updateUserAddressDto)
    {
        try
        {
            var address = await _userAddressRepo.GetById(id);
            if (address == null)
                return NotFound();

            _mapper.Map(updateUserAddressDto, address);
            await _userAddressRepo.Update(address);

            return Ok(_mapper.Map<UserAddressDto>(address));
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