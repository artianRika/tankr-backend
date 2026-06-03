using TankR.Data.Models;
using TankR.Services;
using Xunit;

namespace TankR.Tests;

public class BusinessLogicTests
{
    [Fact]
    public void TotalPrice_IsLitersTimesPricePerLiter()
    {
        var transaction = new Transaction { Liters = 20m, PricePerLiter = 65.50m };

        transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters;

        Assert.Equal(1310.00m, transaction.TotalPrice);
    }

    [Fact]
    public void LoyaltyPoints_IncreaseAfterFuelPurchase()
    {
        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PhoneNumber = "070000000",
            LoyaltyPoints = 100
        };

        user.LoyaltyPoints += LoyaltyRules.PointsForLiters(30m);

        Assert.Equal(160, user.LoyaltyPoints);
    }

    [Fact]
    public void RedeemPoints_100PointsGives10MkdOff()
    {
        var subtotal = 500m;

        var ok = LoyaltyRules.TryRedeem(100, 200, subtotal, out var used, out var discount, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(100, used);
        Assert.Equal(10m, discount);
    }

    [Fact]
    public void RedeemPoints_RejectsInsufficientBalance()
    {
        var ok = LoyaltyRules.TryRedeem(100, 50, 500m, out _, out _, out var error);

        Assert.False(ok);
        Assert.Equal("Insufficient loyalty points.", error);
    }
}
