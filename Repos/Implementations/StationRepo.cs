using TankR.Data;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class StationRepo
{
    private readonly AppDbContext _dbContext;
    public StationRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
}