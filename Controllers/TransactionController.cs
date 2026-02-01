using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Transactions;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IStationRepo _stationRepo;
        private readonly IFuelTypeRepo _fuelTypeRepo;
        private readonly IStationEmployeeRepo _stationEmployeeRepo;
        private readonly IStationFuelPriceRepo _stationFuelPriceRepo;
        private readonly IMapper _mapper;
        
        private readonly EmailSender _email;

        public TransactionController(
            ITransactionRepo transactionRepo, 
            IUserRepo userRepo, 
            IStationRepo stationRepo, 
            IFuelTypeRepo fuelTypeRepo,
            IStationFuelPriceRepo stationFuelPriceRepo,
            IStationEmployeeRepo stationEmployeeRepo,
            IMapper mapper,
            EmailSender email)
        {
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
            _stationRepo = stationRepo;
            _fuelTypeRepo = fuelTypeRepo;
            _stationFuelPriceRepo = stationFuelPriceRepo;
            _stationEmployeeRepo = stationEmployeeRepo;
            _mapper = mapper;
            _email = email;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var transaction = await _transactionRepo.GetById(id);
                if (transaction == null) return NotFound();

                var domainUser = await _userRepo.GetById(transaction.CustomerId);
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityUserId)) return Unauthorized();
                
                if (!string.Equals(domainUser.IdentityUserId, identityUserId, StringComparison.Ordinal))
                    return Forbid(); 
                
                return Ok(_mapper.Map<TransactionDto>(transaction));
              
            }
            catch (Exception e)
            {
                 return Problem(
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {

                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityUserId)) return Unauthorized();

                var domainUser = await _userRepo.GetById(userId);
                if (domainUser == null) return NotFound("User not found");

                if (User.IsInRole("Admin"))
                {
                    var txAdmin = await _transactionRepo.GetByUser(userId);
                    return Ok(_mapper.Map<IEnumerable<TransactionDto>>(txAdmin));
                }

                if (User.IsInRole("Customer"))
                {
                    if (!string.Equals(domainUser.IdentityUserId, identityUserId, StringComparison.Ordinal))
                        return Forbid(); // 403

                    var tx = await _transactionRepo.GetByUser(userId);
                    return Ok(_mapper.Map<IEnumerable<TransactionDto>>(tx));
                }

                return Forbid();
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
        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetByStation(int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return NotFound($"Station with id: {stationId} not found");
            
            var transactions = await _transactionRepo.GetByStation(stationId);
            return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionDto dto)
        {
            try
            {

                var user = await _userRepo.GetById(dto.CustomerId);
                if (user == null) return NotFound("User with id: " + dto.CustomerId + " not found");

                var station = await _stationRepo.GetById(dto.StationId);
                if (station == null) return NotFound("Station with id: " + dto.StationId + " not found");

                var identityCashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityCashierId)) return Unauthorized();
                
                var domainCashier = await _userRepo.GetByIdentityId(identityCashierId);
                if (domainCashier == null) return NotFound("Cashier not found");
                
                var validCashier = await _stationEmployeeRepo.Exists(dto.StationId, domainCashier.Id);
                if (!validCashier) return NotFound("This cashier was not found in that station");
                
                
                var fuelType = await _fuelTypeRepo.GetById(dto.FuelTypeId);
                if (fuelType == null) return NotFound("Fuel type with id: " + dto.FuelTypeId + " not found");


                var pricePerLiter = await _stationFuelPriceRepo.Get(dto.StationId, dto.FuelTypeId);
                if (pricePerLiter == null) return NotFound($"{station.Name} doesn't offer {fuelType.Name}");
                

                var transaction = _mapper.Map<Transaction>(dto);

                transaction.Liters = dto.Liters;
                transaction.PricePerLiter = pricePerLiter.Price;
                transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters;

                transaction.PointsEarned = Convert.ToInt32(dto.Liters) * 2;

                user.LoyaltyPoints += transaction.PointsEarned;
                await _userRepo.Update(user);

                transaction.CashierId = domainCashier.Id;
                
                await _transactionRepo.Add(transaction);
                
                var html = $@"
                    <p>Hello {user.FirstName},</p>
                    <p>Thank you for choosing {transaction.Station.Name}!</p>


                    <h3>⛽ Transaction Details</h3>
                    <ul>
                        <li>🛢<b>Liters:</b>️ {transaction.Liters:N2} of {transaction.FuelType.Name}</li>
                        <li>⭐<b>Points earned:</b> {transaction.PointsEarned} </li>
                        <li>🎉<b>Total points:</b>{user.LoyaltyPoints}</li>
                        <li>💰<b>Total paid:</b>{transaction.TotalPrice:N2} MKD</li>
                    </ul>

                    <p>
                    Warm regards,<br/>
                    <b>The TankR Team 🚀</b>
                    </p>

                    <p style=""font-size: 12px; color: gray;"">
                    This is an automated message. Please do not reply directly to this email.
                    ";
                
                await _email.SendAsync(
                    "artianrika@gmail.com",
                    "Transaction Details",
                   html
                );
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id },
                    _mapper.Map<TransactionDto>(transaction));
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
}
