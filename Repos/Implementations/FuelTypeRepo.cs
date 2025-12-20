using TankR.Data;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class FuelTypeRepo
{
    private readonly AppDbContext _dbContext;
    public FuelTypeRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
}