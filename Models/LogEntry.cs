using System;

namespace DoublonManager.Models
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string User { get; set; }

        public LogEntry(string action, string details)
        {
            Date = DateTime.Now;
            Action = action;
            Details = details;
            User = Environment.UserName;
        }
    }
}
