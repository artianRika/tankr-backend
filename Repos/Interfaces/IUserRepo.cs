using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IUserRepo
{
    Task<User?> GetById(int id);
    Task<User?> GetByIdentityId(string identityId);
    Task<User?> GetByEmail(string email);
    Task<IEnumerable<User>> GetAll();

    Task Add(User user);
    Task Update(User user);
}