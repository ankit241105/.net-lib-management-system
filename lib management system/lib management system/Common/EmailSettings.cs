namespace lib_management_system.Common
{
    // Bound from the "Email" section of appsettings.json. If Host is blank, the email
    // service runs in "log-mode" (writes the email to the log instead of sending it).
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string From { get; set; } = "library@localhost";
        public string FromName { get; set; } = "Library Management";
        public bool UseStartTls { get; set; } = true;

        // A loan is reminded when it is due within this many hours (24 = "1 day or less").
        public int ReminderHours { get; set; } = 24;
    }
}
