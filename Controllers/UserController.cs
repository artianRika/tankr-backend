using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos;
using TankR.Data.Models;
using TankR.Repos.Interfaces;
using TankR.Services;

namespace TankR.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly ITransactionRepo _transactionRepo;
    private readonly IMapper _mapper;

    public UserController(IUserRepo userRepo, ITransactionRepo transactionRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _transactionRepo = transactionRepo;
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
    [HttpGet("me")]
    public async Task<ActionResult<UserDetailsDto>> GetMe()
    {
        try
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var user = await _userRepo.GetByIdentityId(identityUserId);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserDetailsDto>(user));
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
    [HttpGet("me/loyalty")]
    public async Task<ActionResult<LoyaltySummaryDto>> GetMyLoyalty()
    {
        try
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var user = await _userRepo.GetByIdentityId(identityUserId);
            if (user == null)
                return NotFound();

            var transactions = (await _transactionRepo.GetByUser(user.Id))?.ToList() ?? [];

            var history = new List<LoyaltyHistoryEntryDto>();
            foreach (var tx in transactions)
            {
                if (tx.PointsRedeemed > 0)
                {
                    history.Add(new LoyaltyHistoryEntryDto
                    {
                        Type = "Redeemed",
                        Points = tx.PointsRedeemed,
                        DiscountMkd = tx.LoyaltyDiscountMkd,
                        TransactionId = tx.Id,
                        CreatedAt = tx.CreatedAt
                    });
                }

                if (tx.PointsEarned > 0)
                {
                    history.Add(new LoyaltyHistoryEntryDto
                    {
                        Type = "Earned",
                        Points = tx.PointsEarned,
                        TransactionId = tx.Id,
                        CreatedAt = tx.CreatedAt
                    });
                }
            }

            return Ok(new LoyaltySummaryDto
            {
                Balance = user.LoyaltyPoints,
                PointsPerDiscountBlock = LoyaltyRules.PointsPerDiscountBlock,
                DiscountMkdPerBlock = LoyaltyRules.DiscountMkdPerBlock,
                History = history.OrderByDescending(h => h.CreatedAt)
            });
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