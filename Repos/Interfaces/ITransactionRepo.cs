using TankR.Data.Models;

public interface ITransactionRepo
{
    Task<Transaction?> GetById(int id);
    Task<IEnumerable<Transaction>?> GetByStation(int stationId);
    Task<IEnumerable<Transaction>?> GetByUser(int userId);

    Task Add(Transaction transaction);
    Task Update(Transaction transaction);
    Task Delete(Transaction transaction);

}