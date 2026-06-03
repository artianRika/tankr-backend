namespace TankR.Services;

public static class LoyaltyRules
{
    public const int PointsPerLiter = 2;
    public const int PointsPerDiscountBlock = 100;
    public const decimal DiscountMkdPerBlock = 10m;

    public static int PointsForLiters(decimal liters) =>
        Convert.ToInt32(liters) * PointsPerLiter;

    public static decimal DiscountForPoints(int points) =>
        (points / PointsPerDiscountBlock) * DiscountMkdPerBlock;

    public static bool TryRedeem(
        int pointsToRedeem,
        int availableBalance,
        decimal subtotal,
        out int pointsUsed,
        out decimal discountMkd,
        out string? error)
    {
        pointsUsed = 0;
        discountMkd = 0;
        error = null;

        if (pointsToRedeem == 0)
            return true;

        if (pointsToRedeem < PointsPerDiscountBlock)
        {
            error = $"Minimum redemption is {PointsPerDiscountBlock} points.";
            return false;
        }

        if (pointsToRedeem % PointsPerDiscountBlock != 0)
        {
            error = $"Redeem points in multiples of {PointsPerDiscountBlock}.";
            return false;
        }

        if (pointsToRedeem > availableBalance)
        {
            error = "Insufficient loyalty points.";
            return false;
        }

        discountMkd = DiscountForPoints(pointsToRedeem);
        if (discountMkd > subtotal)
        {
            error = "Discount cannot exceed transaction total.";
            return false;
        }

        pointsUsed = pointsToRedeem;
        return true;
    }
}
