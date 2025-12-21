using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IFuelTypeRepo
{
    Task<FuelType?> GetById(int id);
    Task<FuelType?> GetByName(string name);
    Task<IEnumerable<FuelType>> GetAll();

    Task Add(FuelType fuelType);
    Task Update(FuelType fuelType);

}