using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationAddressRepo: IStationAddressRepo
{
    private readonly AppDbContext _dbContext;
    public StationAddressRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<StationAddress?> GetById(int id)
    {
        return await _dbContext.StationAddresses.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<StationAddress>?> GetAll()
    {
        return await _dbContext.StationAddresses.ToListAsync();
    }

    public async Task Add(StationAddress address)
    {
        await _dbContext.StationAddresses.AddAsync(address);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(StationAddress address)
    {
        _dbContext.StationAddresses.Update(address);
        await _dbContext.SaveChangesAsync();
    }
}