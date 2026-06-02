using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Enums;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public static class StationEmployeeSeeder
{
    public static async Task SeedCashiersForAllStationsAsync(
        AppDbContext db,
        List<Station> stations,
        IEnumerable<string> cashierEmails)
    {
        var emails = cashierEmails.ToList();
        if (!emails.Any())
            throw new Exception("No cashier emails provided");

        for (int i = 0; i < stations.Count; i++)
        {
            var station = stations[i];
            var selectedEmail = emails[i % emails.Count];

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Email == selectedEmail);

            if (user == null)
            {
                Console.WriteLine($"[SEED WARNING] Cashier user '{selectedEmail}' not found for station {station.Name}");
                continue;
            }

            if (user.Role != UserRole.Cashier)
            {
                Console.WriteLine($"[SEED WARNING] User '{selectedEmail}' is not a Cashier for station {station.Name}");
                continue;
            }

            var exists = await db.StationEmployees.AnyAsync(se =>
                se.StationId == station.Id &&
                se.UserId == user.Id);

            if (exists)
                continue;

            var stationEmployee = new StationEmployee
            {
                StationId = station.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            db.StationEmployees.Add(stationEmployee);
            await db.SaveChangesAsync();
        }
    }

    public static async Task SeedCashierForStationAsync(
        AppDbContext db,
        int stationId,
        string cashierEmail)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == cashierEmail);

        if (user == null)
            throw new Exception($"Cashier user '{cashierEmail}' not found");

        if (user.Role != UserRole.Cashier)
            throw new Exception($"User '{cashierEmail}' is not a Cashier");

        var exists = await db.StationEmployees.AnyAsync(se =>
            se.StationId == stationId &&
            se.UserId == user.Id);

        if (exists)
            return; 

        var stationEmployee = new StationEmployee
        {
            StationId = stationId,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow  
        };

        db.StationEmployees.Add(stationEmployee);
        await db.SaveChangesAsync();
    }
}
