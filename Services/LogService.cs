using System;
using System.Collections.Generic;
using System.IO;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    public class LogService
    {
        private readonly string _logPath;

        public LogService()
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");
        }

        public void Log(string action, string details)
        {
            var entry = new LogEntry(action, details);
            try
            {
                string line = $"{entry.Date:G}|{entry.User}|{entry.Action}|{entry.Details}{Environment.NewLine}";
                File.AppendAllText(_logPath, line);
            }
            catch (Exception ex)
            {
                // Fallback to console or debug
                System.Diagnostics.Debug.WriteLine($"Log failed: {ex.Message}");
            }
        }

        public List<LogEntry> GetLogs()
        {
            var logs = new List<LogEntry>();
            if (!File.Exists(_logPath)) return logs;

            try
            {
                var lines = File.ReadAllLines(_logPath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        logs.Add(new LogEntry(parts[2], parts[3])
                        {
                            Date = DateTime.Parse(parts[0]),
                            User = parts[1]
                        });
                    }
                }
            }
            catch { }
            return logs;
        }
    }
}
