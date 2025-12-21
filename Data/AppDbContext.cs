using Microsoft.EntityFrameworkCore;
using TankR.Data.Models;

namespace TankR.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<StationAddress> StationAddresses { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<FuelType> FuelTypes { get; set; }
    public DbSet<StationFuelPrice> StationFuelPrices { get; set; }
    public DbSet<StationEmployee> StationEmployees { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<StationPhoto> StationPhotos { get; set; }
    
}