namespace Timora.Data.Entities
{
    public class HolidayRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Default status is "Pending"
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime ResolvedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
