using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IAddressRepo
{
    Task<Address?> GetById(int id);
    Task<IEnumerable<Address>?> GetAll();
    
    Task Update(Address address);
    Task Add(Address address);
}