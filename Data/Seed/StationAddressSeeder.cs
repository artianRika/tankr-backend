using Microsoft.EntityFrameworkCore;
using TankR.Data.Enums;
using TankR.Data.Models;

namespace TankR.Data.Seed;

public class StationAddressSeeder
{
   
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