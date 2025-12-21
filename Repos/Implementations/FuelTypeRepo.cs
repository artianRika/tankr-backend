using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class FuelTypeRepo: IFuelTypeRepo
{
    private readonly AppDbContext _dbContext;
    public FuelTypeRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<FuelType>> GetAll()
    {
        return await _dbContext.FuelTypes.ToListAsync();  //.Include(u => u.Address)
    }

    public async Task<FuelType?> GetById(int id)
    {
        return await _dbContext.FuelTypes
            // .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == id); 
    }

    public async Task<FuelType?> GetByName(string name)
    {
        return await _dbContext.FuelTypes
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task Add(FuelType fuelType)
    {
        await _dbContext.FuelTypes.AddAsync(fuelType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(FuelType fuelType)
    {
        _dbContext.FuelTypes.Update(fuelType);
        await _dbContext.SaveChangesAsync();
    }

}