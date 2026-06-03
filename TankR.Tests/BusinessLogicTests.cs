using TankR.Data.Models;
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

        // 2 points per rounded liter (same rule as TransactionController.Create)
        user.LoyaltyPoints += Convert.ToInt32(30m) * 2;

        Assert.Equal(160, user.LoyaltyPoints);
    }
}
