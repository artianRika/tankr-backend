using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class AddressRepo: IAddressRepo
{
    private readonly AppDbContext _dbContext;
    public AddressRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Address?> GetById(int id)
    {
        return await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Address>?> GetAll()
    {
        return await _dbContext.Addresses.ToListAsync();
    }

    public async Task Add(Address address)
    {
        await _dbContext.Addresses.AddAsync(address);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Address address)
    {
        _dbContext.Addresses.Update(address);
        await _dbContext.SaveChangesAsync();
    }
}