using System.ComponentModel.DataAnnotations;

namespace victoria_tap.Entities
{
    public class DispenserInfo
    {
        [Key]
        public string Id { get; set; }
        public double Amount { get; set; }
        public List<Usages> Usages { get; set; }
        public string DispenserId { get; set; }
    }
}
