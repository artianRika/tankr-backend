using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public static class StationSeeder
{
    public static async Task<List<Station>> SeedStationsAsync(
        AppDbContext db,
        IConfiguration config)
    {
        var stations = new List<Station>();

        var stationNames = new[]
        {
            new { Name = "Test Station", Logo = "https://upload.wikimedia.org/wikipedia/en/thumb/e/e8/Shell_logo.svg/1200px-Shell_logo.svg.png" },
            new { Name = "Downtown Fuel", Logo = "https://upload.wikimedia.org/wikipedia/en/thumb/e/e8/Shell_logo.svg/1200px-Shell_logo.svg.png" },
            new { Name = "Highway Express", Logo = "https://upload.wikimedia.org/wikipedia/en/thumb/e/e8/Shell_logo.svg/1200px-Shell_logo.svg.png" },
            new { Name = "Airport Petrol", Logo = "https://upload.wikimedia.org/wikipedia/en/thumb/e/e8/Shell_logo.svg/1200px-Shell_logo.svg.png" },
        };

        foreach (var stn in stationNames)
        {
            var station = await db.Stations.FirstOrDefaultAsync(s => s.Name == stn.Name);
            if (station == null)
            {
                station = new Station
                {
                    Name = stn.Name,
                    LogoUrl = stn.Logo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Stations.Add(station);
                await db.SaveChangesAsync();
            }

            stations.Add(station);

            // Add fuel types and prices for each station
            await SeedFuelTypesAndPricesAsync(db, station.Id);
        }

        return stations;
    }

    // Since this now returns a list, we need a wrapper for backwards compatibility
    public static async Task<Station> SeedStationAsync(
        AppDbContext db,
        IConfiguration config)
    {
        var stations = await SeedStationsAsync(db, config);
        return stations.FirstOrDefault() ?? new Station { Id = 1 };
    }

    // ---------------- PRIVATE HELPERS ----------------

    private static async Task SeedFuelTypesAndPricesAsync(
        AppDbContext db,
        int stationId)
    {
        var diesel = await EnsureFuelTypeAsync(db, "Diesel");
        var gasoline95 = await EnsureFuelTypeAsync(db, "Gasoline 95");
        var gasoline98 = await EnsureFuelTypeAsync(db, "Gasoline 98");
        var lpg = await EnsureFuelTypeAsync(db, "LPG");

        // Vary prices slightly per station for realism
        var priceVariation = (stationId - 1) * 2m;
        
        await EnsureStationFuelPriceAsync(db, stationId, diesel.Id, 77m + priceVariation);
        await EnsureStationFuelPriceAsync(db, stationId, gasoline95.Id, 72m + priceVariation);
        await EnsureStationFuelPriceAsync(db, stationId, gasoline98.Id, 80m + priceVariation);
        await EnsureStationFuelPriceAsync(db, stationId, lpg.Id, 55m + priceVariation);
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
