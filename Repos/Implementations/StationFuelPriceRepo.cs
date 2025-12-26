using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationFuelPriceRepo: IStationFuelPriceRepo
{
    private readonly AppDbContext _dbContext;
    public StationFuelPriceRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StationFuelPrice?> Get(int stationId, int fuelTypeId)
    {
        return await _dbContext.StationFuelPrices.Where(u => u.StationId == stationId && u.FuelTypeId == fuelTypeId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<StationFuelPrice>?> GetAllByStation(int stationId)
    {
        return await _dbContext.StationFuelPrices.Where(u => u.StationId == stationId).ToListAsync();
    }

    public async Task<IEnumerable<StationFuelPrice>?> GetAll()
    {
        return await _dbContext.StationFuelPrices.ToListAsync();
    }

    public async Task SetPrice(int stationId, int fuelTypeId, decimal newPrice)
    {
        var entity = new StationFuelPrice
        {
            StationId = stationId,
            FuelTypeId = fuelTypeId,
            Price = newPrice
        };

        await _dbContext.StationFuelPrices.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePrice(int stationId, int fuelTypeId, decimal newPrice)
    {
        var entity = await _dbContext.StationFuelPrices
            .FirstOrDefaultAsync(x =>
                x.StationId == stationId &&
                x.FuelTypeId == fuelTypeId);

        if (entity == null)
            throw new KeyNotFoundException("Fuel price not found");

        entity.Price = newPrice;
        await _dbContext.SaveChangesAsync();
    }
}