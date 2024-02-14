using AppSecPracticalAssignment_223981B.Models;

namespace AppSecPracticalAssignment_223981B.Service
{
    public class AuditLogService
    {
        private readonly AuthDbContext _context;

        public AuditLogService(AuthDbContext context)
        {
            _context = context;
        }

        public void Log(string userId, string action, string details)
        {
            var logEntry = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(logEntry);
            _context.SaveChanges();
        }
    }
}
