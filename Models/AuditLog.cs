﻿namespace AppSecPracticalAssignment_223981B.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
