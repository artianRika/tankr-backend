using Microsoft.EntityFrameworkCore;
using TankR.Data.Enums;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public class StationAddressSeeder
{
    public static async Task SeedStationAddressesAsync(
        AppDbContext db,
        IConfiguration config,
        List<Station> stations)
    {
        var addresses = new[]
        {
            new { 
                Street = "Sv. Kiril i Metodij", 
                Number = "4", 
                City = "Skopje", 
                PostalCode = "1000", 
                Lat = 41.990230m, 
                Lng = 21.431633m 
            },
            new { 
                Street = "Marsal Tito", 
                Number = "87", 
                City = "Skopje", 
                PostalCode = "1000", 
                Lat = 41.993821m, 
                Lng = 21.420211m 
            },
            new { 
                Street = "11 Oktober", 
                Number = "15", 
                City = "Kumanovo", 
                PostalCode = "1300", 
                Lat = 42.134186m, 
                Lng = 21.713895m 
            },
            new { 
                Street = "Aeroport Road", 
                Number = "101", 
                City = "Petrovec", 
                PostalCode = "1230", 
                Lat = 41.859638m, 
                Lng = 21.325211m 
            },
        };

        for (int i = 0; i < stations.Count; i++)
        {
            var exists = await db.StationAddresses
                .AnyAsync(a => a.StationId == stations[i].Id);

            if (exists)
                continue;

            var addr = addresses[i % addresses.Length];
            
            var address = new StationAddress
            {
                StationId = stations[i].Id,
                Street = addr.Street,
                StreetNumber = addr.Number,
                City = addr.City,
                PostalCode = addr.PostalCode,
                Country = CountryCode.NMK,
                Lat = addr.Lat,
                Lng = addr.Lng
            };

            db.StationAddresses.Add(address);
            await db.SaveChangesAsync();
        }
    }

    public static async Task SeedStationAddressAsync(
        AppDbContext db,
        IConfiguration config,
        int stationId)
    {
        var exists = await db.StationAddresses
            .AnyAsync(a => a.StationId == stationId);

        if (exists)
            return;

        var address = new StationAddress
        {
            StationId = stationId,
            Street = config["Seed:StationAddress:Street"] ?? "Sv. Kiril i Metodij",
            StreetNumber = config["Seed:StationAddress:Number"] ?? "4",
            City = config["Seed:StationAddress:City"] ?? "Skopje",
            PostalCode = config["Seed:StationAddress:PostalCode"] ?? "1000",
            Country = Enum.TryParse<CountryCode>(
                config["Seed:StationAddress:Country"],
                out var c) ? c : CountryCode.NMK,
            Lat = decimal.TryParse(
                config["Seed:StationAddress:Lat"], out var lat)
                ? lat : 41.990230m,
            Lng = decimal.TryParse(
                config["Seed:StationAddress:Lng"], out var lng)
                ? lng : 21.431633m
        };

        db.StationAddresses.Add(address);
        await db.SaveChangesAsync();
    }
}
