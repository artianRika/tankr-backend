using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Enums;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public static class StationEmployeeSeeder
{
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
