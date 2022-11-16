using victoria_tap.Entities;

namespace victoria_tap.Providers.Contracts
{
    // Interface segregation is key for abstraction, loose dependencies and test-friendly approach
    public interface ISpendingInfoCalculator
    {
        DispenserInfo CalculateClosedTapAmount(Dispenser dispenser, DispenserInfo dispenserInfo);
        DispenserInfo CalculateOpenTapAmount(DispenserInfo dispenserInfo);
    }
}
