using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IFuelTypeRepo
{
    IEnumerable<FuelType> GetAll();
    FuelType GetById(int id);

}