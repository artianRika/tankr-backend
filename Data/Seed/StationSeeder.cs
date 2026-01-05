using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public static class StationSeeder
{
    public static async Task<Station> SeedStationAsync(
        AppDbContext db,
        IConfiguration config)
    {
        var name = config["Seed:StationName"] ?? "Test Station";

        var station = await db.Stations.FirstOrDefaultAsync(s => s.Name == name);
        if (station == null)
        {
            station = new Station
            {
                Name = name,
                LogoUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/e/e8/Shell_logo.svg/1200px-Shell_logo.svg.png"
            };

            db.Stations.Add(station);
            await db.SaveChangesAsync();
        }

        // 👇 ADD FUEL TYPES + PRICES HERE
        await SeedFuelTypesAndPricesAsync(db, station.Id);

        return station;
    }

    // ---------------- PRIVATE HELPERS ----------------

    private static async Task SeedFuelTypesAndPricesAsync(
        AppDbContext db,
        int stationId)
    {
        var diesel = await EnsureFuelTypeAsync(db, "Diesel");
        var gasoline95 = await EnsureFuelTypeAsync(db, "Gasoline 95");

        await EnsureStationFuelPriceAsync(db, stationId, diesel.Id, 77);
        await EnsureStationFuelPriceAsync(db, stationId, gasoline95.Id, 72);
    }

    private static async Task<FuelType> EnsureFuelTypeAsync(
        AppDbContext db,
        string name)
    {
        var existing = await db.FuelTypes.FirstOrDefaultAsync(f => f.Name == name);
        if (existing != null) return existing;

        var now = DateTime.UtcNow;

        var fuelType = new FuelType
        {
            Name = name,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.FuelTypes.Add(fuelType);
        await db.SaveChangesAsync();

        return fuelType;
    }

    private static async Task EnsureStationFuelPriceAsync(
        AppDbContext db,
        int stationId,
        int fuelTypeId,
        decimal price)
    {
        var existing = await db.StationFuelPrices
            .FirstOrDefaultAsync(p =>
                p.StationId == stationId &&
                p.FuelTypeId == fuelTypeId);

        if (existing != null) return;

        var now = DateTime.UtcNow;

        var row = new StationFuelPrice
        {
            StationId = stationId,
            FuelTypeId = fuelTypeId,
            Price = price,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.StationFuelPrices.Add(row);
        await db.SaveChangesAsync();
    }
}
