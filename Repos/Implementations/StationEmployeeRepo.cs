using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationEmployeeRepo: IStationEmployeeRepo
{
    private readonly AppDbContext _dbContext;
    public StationEmployeeRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Exists(int stationId, int userId)
    {
        return await _dbContext.StationEmployees.AnyAsync(a => a.StationId == stationId && a.UserId == userId);
    }

    public async Task Assign(StationEmployee stationEmployee)
    { 
        await _dbContext.StationEmployees.AddAsync(stationEmployee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(int stationId, int userId)
    {
        var entity = await _dbContext.StationEmployees.FirstOrDefaultAsync(a => a.StationId == stationId && a.UserId == userId);

        if (entity != null)
        {
            _dbContext.StationEmployees.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StationEmployee>?> GetEmployeesByStation(int stationId)
    {
        return await _dbContext.StationEmployees.Include(u => u.User).Where(a => a.StationId == stationId).ToListAsync();
    }
}