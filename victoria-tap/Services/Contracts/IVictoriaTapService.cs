namespace victoria_tap.Services.Contracts
{
    // Interface segregation is key for abstraction, loose dependencies and test-friendly approach
    public interface IVictoriaTapService
    {
        VictoriaGenericResponse<CreateDispenserResponse> CreateDispenser(float flowVolume);
        VictoriaGenericResponse<bool> ChangeDispenserStatus(string status, string updatedAt, string id);
        VictoriaGenericResponse<DispenserSpendingInfoResponse> GetDispenserSpendingInfo(string id);
    }
}
