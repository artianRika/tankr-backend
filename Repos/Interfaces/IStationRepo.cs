using TankR.Data.Models;

namespace TankR.Repos.Interfaces;

public interface IStationRepo
{
    Task<Station?> GetById(int id);
    Task<IEnumerable<Station>> GetAll();

    Task Add(Station station);
    Task Update(Station station);

}