using System.ComponentModel.DataAnnotations;
using victoria_tap.Shared;

namespace victoria_tap.Entities
{
    public class Dispenser
    {
        [Key]
        public string Id { get; set; }
        public float FlowVolume { get; set; }
        public DispenserStatus Status { get; set; }
        public string LastUpdatedAt { get; set; }
    }
}
