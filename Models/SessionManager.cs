using System.Collections.Concurrent;

namespace AppSecPracticalAssignment_223981B.Models
{
    public class SessionManager
    {
        private static ConcurrentDictionary<string, string> userSessions = new ConcurrentDictionary<string, string>();

        public static void AddSession(string userId, string sessionId)
        {
            userSessions.AddOrUpdate(userId, sessionId, (key, oldValue) => sessionId);
        }

        public static void RemoveSession(string userId)
        {
            userSessions.TryRemove(userId, out _);
        }

        public static bool IsSessionValid(string userId, string sessionId)
        {
            return userSessions.TryGetValue(userId, out string storedSessionId) 
                && storedSessionId == sessionId;
        }
    }
}
