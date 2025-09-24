using Microsoft.EntityFrameworkCore;
using Timora.Data.Entities;

namespace Timora.Data.Data
{
    public class TimoraDbContext : DbContext
    {
        public TimoraDbContext(DbContextOptions<TimoraDbContext> options)
            : base(options) { }

        public DbSet<Notice> Notices { get; set; } = null!;
        public DbSet<HolidayRequest> HolidayRequests { get; set; } = null!;
    }
}
