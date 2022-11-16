using victoria_tap.Entities;
using victoria_tap.Providers.Contracts;

namespace victoria_tap.Providers
{
    // Potential improvement: decorator, composite patterns for SRP compliance
    // Providers share Adapter pattern behavior: abstracts implementation behind an interface
    public class SpendingInfoCalculator : ISpendingInfoCalculator
    {
        private static readonly double PricePerLiter = 12.25;
        public DispenserInfo CalculateClosedTapAmount(Dispenser dispenser, DispenserInfo dispenserInfo)
        {
            var updatedDispenserInfo = CalculateAmountSpentDuringElapsedSeconds(dispenserInfo, dispenser.LastUpdatedAt, false);
            return updatedDispenserInfo;
        }

        public DispenserInfo CalculateOpenTapAmount(DispenserInfo dispenserInfo)
        {
            var presentTime = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var updatedDispenserInfo = CalculateAmountSpentDuringElapsedSeconds(dispenserInfo, presentTime, true);
            return updatedDispenserInfo;
        }

        private DispenserInfo CalculateAmountSpentDuringElapsedSeconds(DispenserInfo dispenserInfo, string closedAtDate, bool isTapOpened)
        {
            var currentSpendingInfoItem = dispenserInfo.Usages.Where(di => di.ClosedAt == null)
                                                              .First();

            var openedAt = DateTimeOffset.Parse(currentSpendingInfoItem.OpenedAt);
            var closedAt = DateTimeOffset.Parse(closedAtDate);

            var secondsElapsed = (closedAt - openedAt).TotalSeconds;

            var beerFlowStreamed = Math.Round(secondsElapsed * currentSpendingInfoItem.FlowVolume, 2);
            var totalItemSpent = Math.Round(beerFlowStreamed * PricePerLiter, 2);

            if (!isTapOpened)
            {
                currentSpendingInfoItem.ClosedAt = closedAtDate;
            }

            currentSpendingInfoItem.TotalSpent = totalItemSpent;

            var totalUsagesAmount = Math.Round(dispenserInfo.Usages.Sum(usage => usage.TotalSpent), 2);
            dispenserInfo.Amount = totalUsagesAmount;

            return dispenserInfo;
        }
    }
}
