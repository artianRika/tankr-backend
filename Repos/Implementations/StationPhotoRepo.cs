using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationPhotoRepo: IStationPhotoRepo
{
    private readonly AppDbContext _dbContext;

    public StationPhotoRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StationPhoto>?> GetAll()
    {
        return await _dbContext.StationPhotos.Include(p => p.Station).ThenInclude(a => a.Address).ToListAsync();
    }

    public async Task<IEnumerable<StationPhoto>?> GetAllByStationId(int stationId)
    {
        return await  _dbContext.StationPhotos.Where(s => s.StationId == stationId).Include(p => p.Station).ThenInclude(a => a.Address).ToListAsync();
    }

    public async Task<StationPhoto?> GetPhotoById(int photoId)
    {
        return await _dbContext.StationPhotos.Where(a => a.Id == photoId).Include(p => p.Station).ThenInclude(a => a.Address).FirstOrDefaultAsync();
    }

    public async Task SavePhoto(StationPhoto photo)
    {
        await _dbContext.StationPhotos.AddAsync(photo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePhoto(StationPhoto photo)
    {
        _dbContext.StationPhotos.Remove(photo);
        await _dbContext.SaveChangesAsync();
    }
}