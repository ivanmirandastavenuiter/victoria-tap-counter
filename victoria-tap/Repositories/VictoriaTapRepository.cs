using Microsoft.EntityFrameworkCore;
using victoria_tap.Entities;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Shared;

namespace victoria_tap.Repositories
{
    public class VictoriaTapRepository : IVictoriaTapRepository
    {
        private readonly VictoriaDbContext _context;

        public VictoriaTapRepository(VictoriaDbContext context)
        {
            _context = context;
        }

        public Dispenser CreateDispenser(float flowVolume)
        {
            var newDispenser = new Dispenser()
            {
                Id = Guid.NewGuid().ToString(),
                FlowVolume = flowVolume,
                Status = DispenserStatus.Closed,
                LastUpdatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            _context.Dispensers.Add(newDispenser);
            _context.SaveChanges();

            return newDispenser;
        }

        public Dispenser GetDispenserById(string id)
        {
            return _context.Dispensers.FirstOrDefault(di => di.Id.Equals(id));
        }

        public DispenserInfo GetDispenserInfoById(string id)
        {
            return _context.DispensersInfo.Include(di => di.Usages)
                                          .FirstOrDefault(di => di.DispenserId.Equals(id));
        }

        public bool ChangeDispenserStatus(Dispenser dispenser)
        {
            try
            {
                var update = _context.Dispensers.Update(dispenser);
                _context.SaveChanges();
                return update != null;
            } catch (Exception)
            {
                return false;
            }
        }

        public bool AddDispenserSpendingInfoItem(DispenserInfo dispenserInfo)
        {
            try
            {
                dispenserInfo.Id = Guid.NewGuid().ToString();
                var addition = _context.DispensersInfo.Add(dispenserInfo);
                _context.SaveChanges();
                return addition != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateDispenserSpendingInfoItem(DispenserInfo dispenserInfo)
        {
            try
            {
                var update = _context.DispensersInfo.Update(dispenserInfo);
                _context.SaveChanges();
                return update != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
