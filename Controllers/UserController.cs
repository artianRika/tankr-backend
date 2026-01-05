using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly IMapper _mapper;

    public UserController(IUserRepo userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        try
        {
            var users = await _userRepo.GetAll();

            var result = _mapper.Map<IEnumerable<UserDto>>(users);
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

    [Authorize]
    [HttpGet("{email}")]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        try
        {
            var user = await _userRepo.GetByEmail(email);
            if (user == null)
                return NotFound($"User with email {email} not found");
            var result = _mapper.Map<UserDto>(user);

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

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        try
        {
            var user = await _userRepo.GetById(id);
            if (user == null)
                return NotFound($"User with id {id} not found");
            var result = _mapper.Map<UserDto>(user);

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
    public async Task<ActionResult> Add(CreateUserDto createUserDto)
    {
        try
        {
            var user = _mapper.Map<User>(createUserDto);
            await _userRepo.Add(user);


            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                _mapper.Map<UserDto>(user)
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

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userRepo.GetById(id);

            if (user == null)
                return NotFound();
            _mapper.Map(updateUserDto, user);
            await _userRepo.Update(user);

            return Ok(_mapper.Map<UserDto>(user));
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