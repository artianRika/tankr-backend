using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Transactions;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Controllers
{
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

        public TransactionController(
            ITransactionRepo transactionRepo, 
            IUserRepo userRepo, 
            IStationRepo stationRepo, 
            IFuelTypeRepo fuelTypeRepo,
            IStationFuelPriceRepo stationFuelPriceRepo,
            IStationEmployeeRepo stationEmployeeRepo,
            IMapper mapper)
        {
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
            _stationRepo = stationRepo;
            _fuelTypeRepo = fuelTypeRepo;
            _stationFuelPriceRepo = stationFuelPriceRepo;
            _stationEmployeeRepo = stationEmployeeRepo;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionRepo.GetById(id);
            if (transaction == null) return NotFound();
            return Ok(_mapper.Map<TransactionDto>(transaction));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null) return NotFound("User not found");

            var transactions = await _transactionRepo.GetByUser(userId);
            return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetByStation(int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return NotFound($"Station with id: {stationId} not found");
            
            var transactions = await _transactionRepo.GetByStation(stationId);
            return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionDto dto)
        {
            
            var user = await _userRepo.GetById(dto.CustomerId);
            if (user == null) return NotFound("User with id: " + dto.CustomerId + " not found");
            
            var station = await _stationRepo.GetById(dto.StationId);
            if (station == null) return NotFound("Station with id: " + dto.StationId + " not found");

            var validCashier = await _stationEmployeeRepo.Exists(dto.StationId, dto.CashierId);
            if (!validCashier) return NotFound("Cashier not found");
            
            var fuelType = await _fuelTypeRepo.GetById(dto.FuelTypeId);
            if(fuelType == null) return NotFound("Fuel type with id: " + dto.FuelTypeId + " not found");
            
            
            var pricePerLiter = await _stationFuelPriceRepo.Get(dto.StationId, dto.FuelTypeId);
            if(pricePerLiter == null) return NotFound($"{station.Name} doesn't offer {fuelType.Name}");

            
            
            var transaction = _mapper.Map<Transaction>(dto);
            
            transaction.Liters = dto.Liters;
            transaction.PricePerLiter = pricePerLiter.Price;
            transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters;

            transaction.PointsEarned = Convert.ToInt32(dto.Liters) * 2;
            
            user.LoyaltyPoints += transaction.PointsEarned;
            await _userRepo.Update(user);
                
            
            await _transactionRepo.Add(transaction);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, _mapper.Map<TransactionDto>(transaction));
        }
    }
}
