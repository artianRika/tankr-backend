using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IUserAddressRepo
{
    Task<UserAddress?> GetById(int id);
    Task<IEnumerable<UserAddress>?> GetAll();
    
    Task Update(UserAddress address);
    Task Add(UserAddress address);
}