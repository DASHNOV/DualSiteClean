using System;
using System.IO;
using System.Text;

namespace DoublonManager.Helpers
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public static class LogHelper
    {
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DoublonManager",
            "Logs"
        );

        private static readonly object _lockObject = new object();

        static LogHelper()
        {
            // Créer le dossier de logs s'il n'existe pas
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        /// <summary>
        /// Écrit un message dans le fichier de log
        /// </summary>
        public static void Log(LogLevel level, string category, string message, Exception ex = null)
        {
            try
            {
                string logFileName = $"DoublonManager_{DateTime.Now:yyyy-MM-dd}.log";
                string logFilePath = Path.Combine(LogDirectory, logFileName);

                StringBuilder logEntry = new StringBuilder();
                logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level.ToString().ToUpper()}] [{category}]");
                logEntry.AppendLine($"Message: {message}");

                if (ex != null)
                {
                    logEntry.AppendLine($"Exception Type: {ex.GetType().Name}");
                    logEntry.AppendLine($"Exception Message: {ex.Message}");
                    logEntry.AppendLine($"Stack Trace: {ex.StackTrace}");
                    
                    if (ex.InnerException != null)
                    {
                        logEntry.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                        logEntry.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                    }
                }

                logEntry.AppendLine(new string('-', 100));

                // Écriture thread-safe dans le fichier
                lock (_lockObject)
                {
                    File.AppendAllText(logFilePath, logEntry.ToString());
                }

                // Nettoyer les anciens logs (garder seulement les 30 derniers jours)
                CleanOldLogs();
            }
            catch
            {
                // Si le logging échoue, on ne fait rien pour éviter de crasher l'application
            }
        }

        /// <summary>
        /// Log d'information
        /// </summary>
        public static void Info(string category, string message)
        {
            Log(LogLevel.Info, category, message);
        }

        /// <summary>
        /// Log d'avertissement
        /// </summary>
        public static void Warning(string category, string message)
        {
            Log(LogLevel.Warning, category, message);
        }

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public static void Error(string category, string message, Exception ex = null)
        {
            Log(LogLevel.Error, category, message, ex);
        }

        /// <summary>
        /// Log de debug
        /// </summary>
        public static void Debug(string category, string message)
        {
            Log(LogLevel.Debug, category, message);
        }

        /// <summary>
        /// Log spécifique pour les connexions
        /// </summary>
        public static void LogConnection(string server, string database, bool useWindowsAuth, bool success, Exception ex = null)
        {
            string authType = useWindowsAuth ? "Windows Authentication" : "SQL Server Authentication";
            string message = $"Tentative de connexion - Serveur: {server}, Base: {database}, Auth: {authType}, Résultat: {(success ? "SUCCÈS" : "ÉCHEC")}";
            
            if (success)
            {
                Info("CONNECTION", message);
            }
            else
            {
                Error("CONNECTION", message, ex);
            }
        }

        /// <summary>
        /// Retourne le chemin du dossier de logs
        /// </summary>
        public static string GetLogDirectory()
        {
            return LogDirectory;
        }

        /// <summary>
        /// Retourne le chemin du fichier de log du jour
        /// </summary>
        public static string GetTodayLogFile()
        {
            string logFileName = $"DoublonManager_{DateTime.Now:yyyy-MM-dd}.log";
            return Path.Combine(LogDirectory, logFileName);
        }

        /// <summary>
        /// Nettoie les logs de plus de 30 jours
        /// </summary>
        private static void CleanOldLogs()
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(LogDirectory);
                FileInfo[] logFiles = dirInfo.GetFiles("DoublonManager_*.log");

                foreach (FileInfo file in logFiles)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-30))
                    {
                        file.Delete();
                    }
                }
            }
            catch
            {
                // Ignorer les erreurs de nettoyage
            }
        }

        /// <summary>
        /// Ouvre le dossier de logs dans l'explorateur
        /// </summary>
        public static void OpenLogDirectory()
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", LogDirectory);
            }
            catch (Exception ex)
            {
                Error("LOG_HELPER", "Impossible d'ouvrir le dossier de logs", ex);
            }
        }
    }
}
