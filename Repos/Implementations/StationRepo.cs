using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationRepo: IStationRepo
{
    private readonly AppDbContext _dbContext;
    public StationRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<IEnumerable<Station>> GetAll()
    {
        return await _dbContext.Stations.Include(u => u.Address).ToListAsync();
    }

    public async Task<Station?> GetById(int id)
    {
        return await _dbContext.Stations
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == id); 
    }

    public async Task Add(Station station)
    {
        await _dbContext.Stations.AddAsync(station);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Station station)
    {
        _dbContext.Stations.Update(station);
        await _dbContext.SaveChangesAsync();
    }
    
}