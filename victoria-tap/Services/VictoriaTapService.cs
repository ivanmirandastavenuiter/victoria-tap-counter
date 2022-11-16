using victoria_tap.Entities;
using victoria_tap.Providers.Contracts;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Services.Contracts;
using victoria_tap.Shared;

namespace victoria_tap.Services
{
    public class VictoriaTapService : IVictoriaTapService
    {
        private readonly IVictoriaTapRepository _victoriaTapRepository;
        private readonly ISpendingInfoCalculator _spendingInfoCalculator;

        public VictoriaTapService(IVictoriaTapRepository victoriaTapRepository, ISpendingInfoCalculator spendingInfoCalculator)
        {
            _victoriaTapRepository = victoriaTapRepository;
            _spendingInfoCalculator = spendingInfoCalculator;
        }

        public VictoriaGenericResponse<CreateDispenserResponse> CreateDispenser(float flowVolume)
        {
            try
            {
                var newDispenser = _victoriaTapRepository.CreateDispenser(flowVolume);

                var newDispenserResponse = new CreateDispenserResponse()
                {
                    Id = newDispenser.Id,
                    FlowVolume = newDispenser.FlowVolume
                };

                return new VictoriaGenericResponse<CreateDispenserResponse>(newDispenserResponse);
            } 
            catch (Exception ex)
            {
                throw new Exception($"Error while creating new dispenser: {ex.Message}", ex);
            }
        }

        public VictoriaGenericResponse<bool> ChangeDispenserStatus(string status, string updatedAt, string id)
        {
            try
            {
                var response = false;
                var currentDispenser = _victoriaTapRepository.GetDispenserById(id);

                status = status[0].ToString().ToUpper() + status.Substring(1);

                Enum.TryParse(status, out DispenserStatus incomingStatus);

                if (currentDispenser.Status != incomingStatus)
                {
                    currentDispenser.Status = incomingStatus;
                    currentDispenser.LastUpdatedAt = updatedAt;
                    response = _victoriaTapRepository.ChangeDispenserStatus(currentDispenser);
                    UpdateDispenserSpendingInfo(currentDispenser);
                }

                return new VictoriaGenericResponse<bool>(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while changing dispenser status: {ex.Message}", ex);
            }
        }

        public VictoriaGenericResponse<DispenserSpendingInfoResponse> GetDispenserSpendingInfo(string id)
        {
            try
            {
                var dispenserSpendingInfo = _victoriaTapRepository.GetDispenserInfoById(id);

                if (dispenserSpendingInfo == null)
                {
                    var newDispenserSpendingInfo = new DispenserSpendingInfoResponse()
                    {
                        Amount = 0,
                        Usages = new List<UsagesResponse>()
                    };

                    return new VictoriaGenericResponse<DispenserSpendingInfoResponse>(newDispenserSpendingInfo);
                }

                if (dispenserSpendingInfo.Usages.Any(usage => usage.ClosedAt == null))
                {
                    _spendingInfoCalculator.CalculateOpenTapAmount(dispenserSpendingInfo);
                }

                var dispenserSpendingInfoResponse = new DispenserSpendingInfoResponse()
                {
                    Amount = dispenserSpendingInfo.Amount,
                    Usages = dispenserSpendingInfo.Usages.Select(di =>
                    {
                        return new UsagesResponse()
                        {
                            ClosedAt = di.ClosedAt,
                            OpenedAt = di.OpenedAt,
                            TotalSpent = di.TotalSpent,
                            FlowVolume = di.FlowVolume
                        };
                    }).ToList()
                };

                return new VictoriaGenericResponse<DispenserSpendingInfoResponse>(dispenserSpendingInfoResponse);

            } catch (Exception ex)
            {
                throw new Exception($"Error while getting dispenser spending info: {ex.Message}", ex);
            }
        }

        private void UpdateDispenserSpendingInfo(Dispenser dispenser)
        {
            var existingDispenserInfo = _victoriaTapRepository.GetDispenserInfoById(dispenser.Id);

            if (existingDispenserInfo == null)
            {
                var usages = new Usages()
                {
                    FlowVolume = dispenser.FlowVolume,
                    OpenedAt = dispenser.LastUpdatedAt
                };

                var dispenserSpendingInfo = new DispenserInfo()
                {
                    DispenserId = dispenser.Id,
                    Amount = 0,
                    Usages = new List<Usages>() { usages }
                };

                _victoriaTapRepository.AddDispenserSpendingInfoItem(dispenserSpendingInfo);
            }
            else
            {
                if (dispenser.Status == DispenserStatus.Closed)
                {
                    var updatedSpendingInfo = _spendingInfoCalculator.CalculateClosedTapAmount(dispenser, existingDispenserInfo);
                    _victoriaTapRepository.UpdateDispenserSpendingInfoItem(updatedSpendingInfo);
                }
                else
                {
                    var newUsage = new Usages()
                    {
                        FlowVolume = dispenser.FlowVolume,
                        OpenedAt = dispenser.LastUpdatedAt
                    };

                    existingDispenserInfo.Usages.Add(newUsage);
                    _victoriaTapRepository.UpdateDispenserSpendingInfoItem(existingDispenserInfo);
                }
            }
        }
    }
}
