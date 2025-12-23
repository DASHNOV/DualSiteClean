using System;
using System.Windows.Forms;
using DoublonManager.Forms;

namespace DoublonManager
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuration Windows Forms moderne
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Gestionnaire d'exceptions global
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            try
            {
                // Afficher le splash screen
                using (var splash = new SplashScreen())
                {
                    splash.ShowDialog();
                }
                
                // Lancer le formulaire principal
                Application.Run(new FormDoublonManager());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur fatale au démarrage de l'application :\n\n{ex.Message}\n\n" +
                    $"Stack trace :\n{ex.StackTrace}",
                    "Erreur critique",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        
        /// <summary>
        /// Gestionnaire d'exceptions pour les threads d'UI
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LoggerException(e.Exception, "Exception UI Thread");
        }
        
        /// <summary>
        /// Gestionnaire d'exceptions non gérées
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LoggerException(ex, "Exception non gérée");
            }
        }
        
        /// <summary>
        /// Méthode de logging des exceptions
        /// </summary>
        private static void LoggerException(Exception ex, string source)
        {
            string logPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "DoublonManager",
                "Logs",
                $"Error_{DateTime.Now:yyyyMMdd}.log"
            );
            
            try
            {
                // Créer le dossier si nécessaire
                string? dir = System.IO.Path.GetDirectoryName(logPath);
                if (dir != null && !System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                
                // Écrire dans le fichier log
                string logEntry = $@"
================================================================================
Date/Heure : {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Source     : {source}
Message    : {ex.Message}
Type       : {ex.GetType().FullName}
Stack Trace:
{ex.StackTrace}
================================================================================
";
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // Si le logging échoue, on ne peut rien faire de plus
            }
            
            // Afficher à l'utilisateur
            MessageBox.Show(
                $"Une erreur est survenue :\n\n{ex.Message}\n\n" +
                $"Les détails ont été enregistrés dans :\n{logPath}",
                $"Erreur - {source}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
