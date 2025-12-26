using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;

namespace TankR.Repos
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly AppDbContext _context;

        public TransactionRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>?> GetByStation(int stationId)
        {
            return await _context.Transactions
                .Where(t => t.StationId == stationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>?> GetByUser(int userId)
        {
            return await _context.Transactions
                .Where(t => t.CustomerId == userId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetById(int id)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task Update(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}