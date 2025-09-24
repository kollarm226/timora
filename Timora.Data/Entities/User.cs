namespace Timora.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "Employee"; // Default role is "Employee"

        public ICollection<HolidayRequest> HolidayRequests { get; set; } =
            new List<HolidayRequest>();
        public ICollection<Notice> Notices { get; set; } = new List<Notice>();
    }
}
