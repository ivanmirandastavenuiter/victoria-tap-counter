namespace victoria_tap.Controllers.Contracts
{
    public record ChangeDispenserStatusRequest
    {
        public string Status { get; set; }
        public string UpdatedAt { get; set; }
        public string Id { get; set; }
    }
}
