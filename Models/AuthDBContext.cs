using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppSecPracticalAssignment_223981B.Models
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<TwoFA> TwoFactorAuthentication { get; set; }

        private readonly IConfiguration _configuration;
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
