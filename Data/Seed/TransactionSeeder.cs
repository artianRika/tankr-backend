using Microsoft.EntityFrameworkCore;
using TankR.Data.Models;
using TankR.Repos.Interfaces;

namespace TankR.Data.Seed;

public static class TransactionSeeder
{
    public static async Task SeedTransactionsAsync(
        AppDbContext db,
        IUserRepo userRepo,
        IFuelTypeRepo fuelTypeRepo,
        IStationFuelPriceRepo stationFuelPriceRepo)
    {
        // Optionally force reseed via env var
        var forceReseed = string.Equals(Environment.GetEnvironmentVariable("FORCE_RESEED"), "true", StringComparison.OrdinalIgnoreCase);

        if (!forceReseed && await db.Transactions.AnyAsync())
        {
            Console.WriteLine("[SEED] Transactions already exist, skipping...");
            return;
        }

        Console.WriteLine("[SEED] Starting transaction seeding...");

        var cashier = await userRepo.GetByEmail("cashier@test.com");
        if (cashier == null)
        {
            Console.WriteLine("[SEED ERROR] Cashier not found!");
            return;
        }

        if (forceReseed)
        {
            Console.WriteLine("[SEED] FORCE_RESEED=true, clearing existing transactions...");
            try
            {
                db.Transactions.RemoveRange(db.Transactions);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEED ERROR] Failed to clear transactions: {ex.Message}");
            }
        }

        var customers = new[]
        {
            await userRepo.GetByEmail("customer@test.com"),
            await userRepo.GetByEmail("john.doe@test.com"),
            await userRepo.GetByEmail("jane.smith@test.com"),
            await userRepo.GetByEmail("bob.wilson@test.com"),
            await userRepo.GetByEmail("alice.johnson@test.com"),
        }.Where(u => u != null).ToList();

        if (!customers.Any())
        {
            Console.WriteLine("[SEED ERROR] No customers found!");
            return;
        }

        var stations = await db.Stations.ToListAsync();
        var fuelTypes = await db.FuelTypes.ToListAsync();

        Console.WriteLine($"[SEED] Found {stations.Count} stations, {fuelTypes.Count} fuel types, {customers.Count} customers, 1 cashier");

        if (!stations.Any() || !fuelTypes.Any())
        {
            Console.WriteLine("[SEED ERROR] No stations or fuel types found!");
            return;
        }

        var transactions = new List<Transaction>();
        var random = new Random(42);

        // Generate transactions
        int txCount = 0;
        foreach (var station in stations)
        {
            Console.WriteLine($"[SEED] Processing station: {station.Name} (ID: {station.Id})");
            
            for (int day = -30; day <= 0; day++)
            {
                for (int i = 0; i < 2; i++)
                {
                    var customer = customers[random.Next(customers.Count)];
                    var fuelType = fuelTypes[random.Next(fuelTypes.Count)];
                    
                    // Get price from DB directly
                    var price = await db.StationFuelPrices
                        .Where(p => p.StationId == station.Id && p.FuelTypeId == fuelType.Id)
                        .Select(p => p.Price)
                        .FirstOrDefaultAsync();

                    if (price == 0)
                    {
                        Console.WriteLine($"[SEED WARNING] No price found for station {station.Id}, fuel type {fuelType.Id}");
                        continue;
                    }

                    var liters = (decimal)(random.Next(20, 80) + random.NextDouble());
                    var transaction = new Transaction
                    {
                        StationId = station.Id,
                        CashierId = cashier.Id,
                        CustomerId = customer.Id,
                        FuelTypeId = fuelType.Id,
                        Liters = liters,
                        PricePerLiter = price,
                        TotalPrice = price * liters,
                        PointsEarned = (int)(liters * 2),
                        CreatedAt = DateTime.UtcNow.AddDays(day)
                    };

                    transactions.Add(transaction);
                    txCount++;
                }
            }
        }

        Console.WriteLine($"[SEED] Created {txCount} transactions total");

        if (transactions.Any())
        {
            try
            {
                await db.Transactions.AddRangeAsync(transactions);
                await db.SaveChangesAsync();
                Console.WriteLine($"[SEED] Successfully saved {txCount} transactions to database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEED ERROR] Failed to save transactions: {ex.Message}");
                // attempt to save one by one to surface errors
                foreach (var tx in transactions)
                {
                    try
                    {
                        await db.Transactions.AddAsync(tx);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"[SEED ERROR] Failed to save single transaction (station {tx.StationId}): {ex2.Message}");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("[SEED ERROR] No transactions were created!");
        }
    }
}
