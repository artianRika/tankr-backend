using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class UserAddressRepo: IUserAddressRepo
{
    private readonly AppDbContext _dbContext;
    public UserAddressRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<UserAddress?> GetById(int id)
    {
        return await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<UserAddress>?> GetAll()
    {
        return await _dbContext.UserAddresses.ToListAsync();
    }

    public async Task Add(UserAddress address)
    {
        await _dbContext.UserAddresses.AddAsync(address);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(UserAddress address)
    {
        _dbContext.UserAddresses.Update(address);
        await _dbContext.SaveChangesAsync();
    }
}