using TankR.Data.Models;

public interface ITransactionRepo
{
    Transaction GetById(int id);
    IEnumerable<Transaction> GetByStation(int stationId);

    void Add(Transaction transaction);

}