using victoria_tap.Entities;

namespace victoria_tap.Repositories.Contracts
{
    // Interface segregation is key for abstraction, loose dependencies and test-friendly approach
    public interface IVictoriaTapRepository
    {
        Dispenser GetDispenserById(string id);
        DispenserInfo GetDispenserInfoById(string id);
        Dispenser CreateDispenser(float flowVolume);
        bool ChangeDispenserStatus(Dispenser dispenser);
        bool AddDispenserSpendingInfoItem(DispenserInfo dispenserInfo);
        bool UpdateDispenserSpendingInfoItem(DispenserInfo dispenserInfo);
    }
}
