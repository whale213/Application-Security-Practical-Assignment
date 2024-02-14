namespace AppSecPracticalAssignment_223981B.Models
{
    public class Recaptcha
    {
        public bool Success { get; set; }
        public string Challenge { get; set; }
        public string Hostname { get; set; }
        public List<string> ErrorCodes { get; set; }
    }
}
