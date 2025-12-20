using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Repos.Implementations;

public class UserRepo: IUserRepo
{
    private readonly AppDbContext _dbContext;

    public UserRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _dbContext.Users.Include(u => u.Address).ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        return await _dbContext.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == id); 
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dbContext.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}