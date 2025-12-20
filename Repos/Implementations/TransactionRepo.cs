using TankR.Data;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class TransactionRepo
{
    private readonly AppDbContext _dbContext;
    public TransactionRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
}