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
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("VotreCle32Caracteres12345678!!"); // 32 caractères
        private static readonly byte[] IV = new byte[16]; // IV de 16 bytes (zéros)

        public static string BuildConnectionString(
            string server, 
            string database, 
            bool useWindowsAuth, 
            string username = "", 
            string password = "")
        {
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
            }
            else
            {
                builder.UserID = username;
                builder.Password = password;
            }

            return builder.ConnectionString;
        }

        public static async Task<bool> TestConnection(string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur de connexion :\n{ex.Message}", 
                    "Échec", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return false;
            }
        }

        public static void SaveConnectionSettings(
            ConnectionSettings site39C, 
            ConnectionSettings site19M)
        {
            var settings = new 
            { 
                Site39C = site39C, 
                Site19M = site19M 
            };
            
            string json = JsonConvert.SerializeObject(settings);
            string encrypted = EncryptString(json);
            
            File.WriteAllText(CONFIG_FILE, encrypted);
        }

        public static (ConnectionSettings site39C, ConnectionSettings site19M) LoadConnectionSettings()
        {
            if (!File.Exists(CONFIG_FILE))
                return (null, null);

            try
            {
                string encrypted = File.ReadAllText(CONFIG_FILE);
                string json = DecryptString(encrypted);
                
                dynamic settings = JsonConvert.DeserializeObject(json);
                
                return (
                    JsonConvert.DeserializeObject<ConnectionSettings>(settings.Site39C.ToString()), 
                    JsonConvert.DeserializeObject<ConnectionSettings>(settings.Site19M.ToString())
                );
            }
            catch
            {
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
