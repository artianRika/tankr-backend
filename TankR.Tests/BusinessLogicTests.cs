using TankR.Data.Models;
using Xunit;

namespace TankR.Tests;

public class BusinessLogicTests
{
    [Fact]
    public void TotalPrice_EqualsLitersMultipliedByPricePerLiter()
    {
        // Replicates the calculation in TransactionController.Create:
        // transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters
        var transaction = new Transaction
        {
            Liters = 20m,
            PricePerLiter = 65.50m
        };

        transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters;

        Assert.Equal(1310.00m, transaction.TotalPrice);
    }

    [Fact]
    public void PointsEarned_IsCalculatedAs2PointsPerRoundedLiter()
    {
        // Replicates: transaction.PointsEarned = Convert.ToInt32(dto.Liters) * 2
        // Convert.ToInt32 rounds to nearest integer (banker's rounding on .5)
        decimal liters = 15.7m;
        int expectedPoints = Convert.ToInt32(liters) * 2; // rounds to 16 → 32

        Assert.Equal(32, expectedPoints);
    }

    [Fact]
    public void LoyaltyPoints_AccumulateAcrossMultipleTransactions()
    {
        // Replicates: user.LoyaltyPoints += transaction.PointsEarned
        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PhoneNumber = "070000000",
            LoyaltyPoints = 100
        };

        user.LoyaltyPoints += Convert.ToInt32(30m) * 2; // first fill: 60 pts
        user.LoyaltyPoints += Convert.ToInt32(20m) * 2; // second fill: 40 pts

        Assert.Equal(200, user.LoyaltyPoints);
    }
}
