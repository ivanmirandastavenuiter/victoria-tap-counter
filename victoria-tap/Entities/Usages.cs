using System.ComponentModel.DataAnnotations;

namespace victoria_tap.Entities
{
    public class Usages
    {
        [Key]
        public int Id { get; set; }
        public string OpenedAt { get; set; }
        public string ClosedAt { get; set; }
        public float FlowVolume { get; set; }
        public double TotalSpent { get; set; }
    }
}
