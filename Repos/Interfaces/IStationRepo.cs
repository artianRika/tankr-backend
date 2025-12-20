using TankR.Data.Models;

public interface IStationRepo
{
    Station GetById(int id);
    IEnumerable<Station> GetAll();

    void Add(Station station);
    void Update(Station station);

}