using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms; // Added for MessageBox, although helper ideally shouldn't have UI logic, user asked for it.

namespace DoublonManager.Helpers
{
    public class ConnectionHelper
    {
        private const string CONFIG_FILE = "appsettings.encrypted";
        // Clé AES de 32 bytes (256 bits) - exactement 32 caractères
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 caractères = 32 bytes
        // Vecteur d'initialisation de 16 bytes (128 bits)
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 caractères = 16 bytes


        public static string BuildConnectionString(
            string server, 
            string database, 
            bool useWindowsAuth, 
            string username = "", 
            string password = "")
        {
            LogHelper.Info("CONNECTION_BUILDER", 
                $"Construction de la chaîne de connexion - Serveur: {server}, Base: {database}, " +
                $"Auth: {(useWindowsAuth ? "Windows" : "SQL Server")}, " +
                $"Username: {(string.IsNullOrEmpty(username) ? "N/A" : username)}");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                ConnectTimeout = 30,
                Encrypt = false
            };

            if (useWindowsAuth)
            {
                builder.IntegratedSecurity = true;
                LogHelper.Debug("CONNECTION_BUILDER", "Utilisation de l'authentification Windows intégrée");
            }
            else
            {
                builder.UserID = username;
                builder.Password = password;
                LogHelper.Debug("CONNECTION_BUILDER", $"Utilisation de l'authentification SQL Server avec l'utilisateur: {username}");
            }

            LogHelper.Debug("CONNECTION_BUILDER", "Chaîne de connexion construite avec succès");
            return builder.ConnectionString;
        }

        public static async Task<bool> TestConnection(string connectionString)
        {
            // Extraire le serveur et la base de données pour le logging (sans exposer les credentials)
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string server = builder.DataSource;
            string database = builder.InitialCatalog;
            bool useWindowsAuth = builder.IntegratedSecurity;

            LogHelper.Info("CONNECTION_TEST", 
                $"Début du test de connexion - Serveur: {server}, Base: {database}, " +
                $"Auth: {(useWindowsAuth ? "Windows" : "SQL Server")}, Timeout: {builder.ConnectTimeout}s");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    LogHelper.Debug("CONNECTION_TEST", "Tentative d'ouverture de la connexion...");
                    await conn.OpenAsync();
                    
                    LogHelper.Info("CONNECTION_TEST", 
                        $"✅ Connexion réussie - Serveur: {server}, Base: {database}, Version SQL: {conn.ServerVersion}");
                    
                    LogHelper.LogConnection(server, database, useWindowsAuth, true);
                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                LogHelper.Error("CONNECTION_TEST", 
                    $"❌ Erreur SQL lors de la connexion - Serveur: {server}, Base: {database}, " +
                    $"Code erreur: {sqlEx.Number}, État: {sqlEx.State}, Ligne: {sqlEx.LineNumber}", 
                    sqlEx);
                
                LogHelper.LogConnection(server, database, useWindowsAuth, false, sqlEx);
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CONNECTION_TEST", 
                    $"❌ Erreur générale lors de la connexion - Serveur: {server}, Base: {database}", 
                    ex);
                
                LogHelper.LogConnection(server, database, useWindowsAuth, false, ex);
                return false;
            }
        }

        public static void SaveConnectionSettings(
            ConnectionSettings site39C, 
            ConnectionSettings site19M)
        {
            LogHelper.Info("CONNECTION_SETTINGS", "Début de la sauvegarde des paramètres de connexion");
            try
            {
                var settings = new 
                { 
                    Site39C = site39C, 
                    Site19M = site19M 
                };
                
                string json = JsonConvert.SerializeObject(settings);
                LogHelper.Debug("CONNECTION_SETTINGS", "Sérialisation JSON réussie");

                string encrypted = EncryptString(json);
                LogHelper.Debug("CONNECTION_SETTINGS", "Chiffrement des données réussi");
                
                File.WriteAllText(CONFIG_FILE, encrypted);
                LogHelper.Info("CONNECTION_SETTINGS", $"✅ Paramètres sauvegardés avec succès dans {CONFIG_FILE}");
            }
            catch (Exception ex)
            {
                LogHelper.Error("CONNECTION_SETTINGS", "❌ Erreur lors de la sauvegarde des paramètres de connexion", ex);
                throw;
            }
        }

        public static (ConnectionSettings site39C, ConnectionSettings site19M) LoadConnectionSettings()
        {
            LogHelper.Info("CONNECTION_SETTINGS", "Tentative de chargement des paramètres de connexion");
            
            if (!File.Exists(CONFIG_FILE))
            {
                LogHelper.Warning("CONNECTION_SETTINGS", $"Le fichier de configuration {CONFIG_FILE} n'existe pas encore");
                return (null, null);
            }

            try
            {
                string encrypted = File.ReadAllText(CONFIG_FILE);
                LogHelper.Debug("CONNECTION_SETTINGS", "Lecture du fichier chiffré réussie");

                string json = DecryptString(encrypted);
                LogHelper.Debug("CONNECTION_SETTINGS", "Déchiffrement réussi");
                
                dynamic settings = JsonConvert.DeserializeObject(json);
                
                var site39C = JsonConvert.DeserializeObject<ConnectionSettings>(settings.Site39C.ToString());
                var site19M = JsonConvert.DeserializeObject<ConnectionSettings>(settings.Site19M.ToString());

                LogHelper.Info("CONNECTION_SETTINGS", "✅ Paramètres de connexion chargés avec succès depuis le fichier");
                return (site39C, site19M);
            }
            catch (Exception ex)
            {
                LogHelper.Error("CONNECTION_SETTINGS", "❌ Erreur critique lors du chargement des paramètres de connexion", ex);
                return (null, null);
            }
        }

        private static string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] encrypted = encryptor.TransformFinalBlock(
                    Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
                
                return Convert.ToBase64String(encrypted);
            }
        }

        private static string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                
                byte[] buffer = Convert.FromBase64String(cipherText);
                ICryptoTransform decryptor = aes.CreateDecryptor();
                byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }

    public class ConnectionSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public bool UseWindowsAuth { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
