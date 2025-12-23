using System;
using System.Configuration;
using System.IO;

namespace DoublonManager.Helpers
{
    /// <summary>
    /// Classe helper pour gérer la configuration de l'application
    /// </summary>
    public static class ConfigManager
    {
        #region Connexions
        
        public static string GetConnectionString39C()
        {
            return ConfigurationManager.AppSettings["ConnectionString39C"] 
                ?? "Server=SERVER_39C;Database=Amadeus;Integrated Security=true;";
        }
        
        public static string GetConnectionString19M()
        {
            return ConfigurationManager.AppSettings["ConnectionString19M"] 
                ?? "Server=SERVER_19M;Database=Amadeus;Integrated Security=true;";
        }
        
        #endregion
        
        #region Chemins
        
        public static string GetCheminSauvegardes()
        {
            string chemin = ConfigurationManager.AppSettings["CheminSauvegardes"];
            
            if (string.IsNullOrWhiteSpace(chemin))
            {
                chemin = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "DoublonManager",
                    "Sauvegardes"
                );
            }
            
            try
            {
                // Créer le dossier s'il n'existe pas
                if (!Directory.Exists(chemin))
                {
                    Directory.CreateDirectory(chemin);
                }
            }
            catch { /* Ignorer les erreurs de création ici */ }
            
            return chemin;
        }
        
        public static string GetCheminLogs()
        {
            string chemin = ConfigurationManager.AppSettings["CheminLogs"];
            
            if (string.IsNullOrWhiteSpace(chemin))
            {
                chemin = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "DoublonManager",
                    "Logs"
                );
            }
            
            try
            {
                if (!Directory.Exists(chemin))
                {
                    Directory.CreateDirectory(chemin);
                }
            }
            catch { /* Ignorer les erreurs de création ici */ }
            
            return chemin;
        }
        
        #endregion
        
        #region Options d'analyse
        
        public static int GetSeuilCompletudeMinimum()
        {
            string valeur = ConfigurationManager.AppSettings["SeuilCompletudeMinimum"];
            return int.TryParse(valeur, out int seuil) ? seuil : 50;
        }
        
        public static bool GetPrioritePhoto()
        {
            string valeur = ConfigurationManager.AppSettings["PrioritePhoto"];
            return bool.TryParse(valeur, out bool priorite) ? priorite : true;
        }
        
        #endregion
        
        #region Options de sécurité
        
        public static bool GetConfirmerAvantSuppression()
        {
            string valeur = ConfigurationManager.AppSettings["ConfirmerAvantSuppression"];
            return bool.TryParse(valeur, out bool confirmer) ? confirmer : true;
        }
        
        public static bool GetCreerSauvegardeAvantSuppression()
        {
            string valeur = ConfigurationManager.AppSettings["CreerSauvegardeAvantSuppression"];
            return bool.TryParse(valeur, out bool creer) ? creer : true;
        }
        
        #endregion
        
        #region Options d'interface
        
        public static bool GetAfficherSplashScreen()
        {
            string valeur = ConfigurationManager.AppSettings["AfficherSplashScreen"];
            return bool.TryParse(valeur, out bool afficher) ? afficher : true;
        }
        
        public static int GetDureeSplashScreen()
        {
            string valeur = ConfigurationManager.AppSettings["DureeSplashScreen"];
            return int.TryParse(valeur, out int duree) ? duree : 2000;
        }
        
        #endregion
        
        #region Méthodes de sauvegarde
        
        public static void SaveConnectionString39C(string connectionString)
        {
            SaveSetting("ConnectionString39C", connectionString);
        }
        
        public static void SaveConnectionString19M(string connectionString)
        {
            SaveSetting("ConnectionString19M", connectionString);
        }
        
        private static void SaveSetting(string key, string value)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                if (config.AppSettings.Settings[key] == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception)
            {
                // Gérer l'erreur si nécessaire
            }
        }
        
        #endregion
    }
}
