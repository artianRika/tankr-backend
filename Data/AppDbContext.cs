using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TankR.Data.Models;
using TankR.Data.Models.Identity;

namespace TankR.Data;

public class AppDbContext: IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.HasKey((l => new { l.LoginProvider, l.ProviderKey }));
        });
    }
    
}