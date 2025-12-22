using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DoublonManager.Helpers;

namespace DoublonManager.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString39C;
        private readonly string _connectionString19M;

        public DatabaseService(string connStr39C, string connStr19M)
        {
            _connectionString39C = connStr39C;
            _connectionString19M = connStr19M;
        }

        #region Méthodes de comptage

        /// <summary>
        /// Obtient le nombre total d'employés pour un site
        /// </summary>
        public async Task<int> GetEmployeeCount(string siteCode)
        {
            string connStr = siteCode == "39C" ? _connectionString39C : _connectionString19M;
            LogHelper.Info("DATABASE", $"Comptage des employés pour le site {siteCode}");

            string query = @"
                SELECT COUNT(*) 
                FROM dbo.Cardholders 
                WHERE Status = 1";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    LogHelper.Debug("DATABASE", $"Ouverture de la connexion pour le site {siteCode}...");
                    await conn.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = await cmd.ExecuteScalarAsync();
                        int count = Convert.ToInt32(result);
                        LogHelper.Info("DATABASE", $"Succès : {count} employés actifs trouvés pour le site {siteCode}");
                        return count;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("DATABASE", $"❌ Erreur lors du comptage des employés du site {siteCode}", ex);
                throw new Exception($"Erreur lors du comptage des employés du site {siteCode}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtient les statistiques des deux sites
        /// </summary>
        public async Task<SiteStatistics> GetSiteStatistics()
        {
            var stats = new SiteStatistics();

            try
            {
                // Exécuter les deux requêtes en parallèle
                var task39C = GetEmployeeCount("39C");
                var task19M = GetEmployeeCount("19M");

                await Task.WhenAll(task39C, task19M);

                stats.Count39C = task39C.Result;
                stats.Count19M = task19M.Result;
                stats.LastUpdate = DateTime.Now;

                return stats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes de chargement des employés

        /// <summary>
        /// Charge tous les employés d'un site
        /// </summary>
        public async Task<List<DbEmployee>> GetAllEmployees(string siteCode)
        {
            string connStr = siteCode == "39C" ? _connectionString39C : _connectionString19M;
            LogHelper.Info("DATABASE", $"Chargement de tous les employés actifs pour le site {siteCode}");

            string query = @"
                SELECT 
                    ID,
                    LastName,
                    FirstName,
                    CardholderIdNumber,
                    FromDateValid,
                    Status,
                    DepartmentUID,
                    LastDownloadTime,
                    AD_Username
                FROM dbo.Cardholders
                WHERE Status = 1
                ORDER BY LastName, FirstName";

            List<DbEmployee> employees = new List<DbEmployee>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    LogHelper.Debug("DATABASE", $"Ouverture de la connexion pour le site {siteCode}...");
                    await conn.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new DbEmployee
                                {
                                    SiteCode = siteCode,
                                    ID = reader["ID"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    CardholderIdNumber = reader["CardholderIdNumber"] != DBNull.Value 
                                        ? reader["CardholderIdNumber"].ToString() : "",
                                    FromDateValid = reader["FromDateValid"] != DBNull.Value 
                                        ? Convert.ToDateTime(reader["FromDateValid"]) : DateTime.MinValue,
                                    Status = Convert.ToInt32(reader["Status"]),
                                    DepartmentUID = reader["DepartmentUID"].ToString(),
                                    LastDownloadTime = reader["LastDownloadTime"] != DBNull.Value 
                                        ? Convert.ToDateTime(reader["LastDownloadTime"]) : DateTime.MinValue,
                                    AD_Username = reader["AD_Username"].ToString()
                                });
                            }
                        }
                    }
                }

                LogHelper.Info("DATABASE", $"✅ Chargement terminé : {employees.Count} employés récupérés pour le site {siteCode}");
                return employees;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DATABASE", $"❌ Erreur lors du chargement des employés du site {siteCode}", ex);
                throw new Exception($"Erreur lors du chargement des employés du site {siteCode}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Charge les employés des deux sites en parallèle
        /// </summary>
        public async Task<(List<DbEmployee> site39C, List<DbEmployee> site19M)> GetAllEmployeesFromBothSites()
        {
            try
            {
                var task39C = GetAllEmployees("39C");
                var task19M = GetAllEmployees("19M");

                await Task.WhenAll(task39C, task19M);

                return (task39C.Result, task19M.Result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement des employés des deux sites: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes de détection des doublons

        /// <summary>
        /// Détecte les doublons entre les deux sites
        /// </summary>
        public async Task<DuplicateAnalysisResult> DetectDuplicates()
        {
            LogHelper.Info("ANALYSIS", "Début de la détection des doublons sur les deux sites");
            var result = new DuplicateAnalysisResult
            {
                AnalysisDate = DateTime.Now
            };

            try
            {
                // Charger les employés des deux sites
                var (employees39C, employees19M) = await GetAllEmployeesFromBothSites();

                result.TotalEmployees39C = employees39C.Count;
                result.TotalEmployees19M = employees19M.Count;

                LogHelper.Debug("ANALYSIS", "Lancement de la détection par codes différents...");
                result.DifferentCodeDuplicates = DetectDifferentCodeDuplicates(employees39C, employees19M);
                
                LogHelper.Debug("ANALYSIS", "Lancement de la détection par numéros différents...");
                result.DifferentNumberDuplicates = DetectDifferentNumberDuplicates(employees39C, employees19M);
                
                LogHelper.Debug("ANALYSIS", "Lancement de la détection des cas ambigus...");
                result.AmbiguousCases = DetectAmbiguousCases(employees39C, employees19M);

                LogHelper.Info("ANALYSIS", 
                    $"✅ Analyse terminée. Doublons trouvés : " +
                    $"{result.DifferentCodeDuplicates.Count} codes diff, " +
                    $"{result.DifferentNumberDuplicates.Count} numéros diff, " +
                    $"{result.AmbiguousCases.Count} ambigus");

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ANALYSIS", "❌ Erreur critique lors de la détection des doublons", ex);
                throw new Exception($"Erreur lors de la détection des doublons: {ex.Message}", ex);
            }
        }

        private List<DuplicatePair> DetectDifferentCodeDuplicates(
            List<DbEmployee> site39C, 
            List<DbEmployee> site19M)
        {
            var duplicates = new List<DuplicatePair>();

            foreach (var emp39C in site39C)
            {
                foreach (var emp19M in site19M)
                {
                    // Même nom, prénom et numéro, mais codes différents
                    if (emp39C.LastName.Equals(emp19M.LastName, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.FirstName.Equals(emp19M.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.CardholderIdNumber == emp19M.CardholderIdNumber &&
                        !string.IsNullOrEmpty(emp39C.CardholderIdNumber) &&
                        !emp39C.ID.Equals(emp19M.ID, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicates.Add(new DuplicatePair
                        {
                            Employee39C = emp39C,
                            Employee19M = emp19M,
                            DuplicateType = DbDuplicateType.DifferentCode,
                            Confidence = CalculateConfidence(emp39C, emp19M)
                        });
                    }
                }
            }

            return duplicates;
        }

        private List<DuplicatePair> DetectDifferentNumberDuplicates(
            List<DbEmployee> site39C, 
            List<DbEmployee> site19M)
        {
            var duplicates = new List<DuplicatePair>();

            foreach (var emp39C in site39C)
            {
                foreach (var emp19M in site19M)
                {
                    // Même nom, prénom et code, mais numéros différents
                    if (emp39C.LastName.Equals(emp19M.LastName, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.FirstName.Equals(emp19M.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.ID.Equals(emp19M.ID, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.CardholderIdNumber != emp19M.CardholderIdNumber &&
                        !string.IsNullOrEmpty(emp39C.CardholderIdNumber) &&
                        !string.IsNullOrEmpty(emp19M.CardholderIdNumber))
                    {
                        duplicates.Add(new DuplicatePair
                        {
                            Employee39C = emp39C,
                            Employee19M = emp19M,
                            DuplicateType = DbDuplicateType.DifferentNumber,
                            Confidence = CalculateConfidence(emp39C, emp19M)
                        });
                    }
                }
            }

            return duplicates;
        }

        private List<DuplicatePair> DetectAmbiguousCases(
            List<DbEmployee> site39C, 
            List<DbEmployee> site19M)
        {
            var ambiguous = new List<DuplicatePair>();

            foreach (var emp39C in site39C)
            {
                foreach (var emp19M in site19M)
                {
                    // Même nom et prénom, mais différences sur code ET numéro
                    if (emp39C.LastName.Equals(emp19M.LastName, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.FirstName.Equals(emp19M.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        !emp39C.ID.Equals(emp19M.ID, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.CardholderIdNumber != emp19M.CardholderIdNumber)
                    {
                        // Cas ambigu si dates de modification identiques
                        if (Math.Abs((emp39C.LastDownloadTime - emp19M.LastDownloadTime).TotalHours) < 1)
                        {
                            ambiguous.Add(new DuplicatePair
                            {
                                Employee39C = emp39C,
                                Employee19M = emp19M,
                                DuplicateType = DbDuplicateType.Ambiguous,
                                Confidence = CalculateConfidence(emp39C, emp19M)
                            });
                        }
                    }
                }
            }

            return ambiguous;
        }

        private double CalculateConfidence(DbEmployee emp1, DbEmployee emp2)
        {
            double confidence = 0.0;

            // Nom et prénom identiques : +40%
            if (emp1.LastName.Equals(emp2.LastName, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;
            if (emp1.FirstName.Equals(emp2.FirstName, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;

            // Code identique : +20%
            if (emp1.ID.Equals(emp2.ID, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;

            // Numéro identique : +20%
            if (emp1.CardholderIdNumber == emp2.CardholderIdNumber && !string.IsNullOrEmpty(emp1.CardholderIdNumber))
                confidence += 0.20;

            // Département identique : +10%
            if (emp1.DepartmentUID.Equals(emp2.DepartmentUID, StringComparison.OrdinalIgnoreCase))
                confidence += 0.10;

            // Date d'embauche proche (±30 jours) : +10%
            if (Math.Abs((emp1.FromDateValid - emp2.FromDateValid).TotalDays) <= 30)
                confidence += 0.10;

            return Math.Min(confidence, 1.0); // Max 100%
        }

        #endregion

        #region Méthodes de suppression

        /// <summary>
        /// Supprime un employé d'un site
        /// </summary>
        public async Task<bool> DeleteEmployee(string siteCode, string codeEmploye, string deletedBy)
        {
            string connStr = siteCode == "39C" ? _connectionString39C : _connectionString19M;
            LogHelper.Info("DATABASE", $"Suppression de l'employé {codeEmploye} sur le site {siteCode} (par: {deletedBy})");

            // Option 1 : Suppression logique (statut = 0 ou 2 selon le système)
            string query = @"
                UPDATE dbo.Cardholders 
                SET 
                    Status = 0,
                    LastDownloadTime = GETDATE(),
                    AD_Username = @deletedBy
                WHERE ID = @codeEmploye";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    LogHelper.Debug("DATABASE", $"Ouverture de la connexion pour suppression sur le site {siteCode}...");
                    await conn.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codeEmploye", codeEmploye);
                        cmd.Parameters.AddWithValue("@deletedBy", deletedBy);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        bool success = rowsAffected > 0;
                        
                        if (success)
                            LogHelper.Info("DATABASE", $"✅ Employé {codeEmploye} supprimé avec succès sur le site {siteCode}");
                        else
                            LogHelper.Warning("DATABASE", $"⚠️ Aucune ligne affectée lors de la suppression de {codeEmploye} sur {siteCode}");
                        
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("DATABASE", $"❌ Erreur lors de la suppression de l'employé {codeEmploye} du site {siteCode}", ex);
                throw new Exception($"Erreur lors de la suppression de l'employé {codeEmploye} du site {siteCode}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprime plusieurs employés en batch
        /// </summary>
        public async Task<BatchDeleteResult> DeleteEmployeesBatch(List<DeletionRequest> requests, string deletedBy)
        {
            var result = new BatchDeleteResult
            {
                TotalRequests = requests.Count,
                StartTime = DateTime.Now
            };

            foreach (var request in requests)
            {
                try
                {
                    bool success = await DeleteEmployee(request.SiteCode, request.ID, deletedBy);
                    
                    if (success)
                    {
                        result.SuccessCount++;
                        result.SuccessfulDeletions.Add(request);
                    }
                    else
                    {
                        result.FailureCount++;
                        result.FailedDeletions.Add(new DeletionError
                        {
                            Request = request,
                            ErrorMessage = "Aucune ligne affectée"
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.FailedDeletions.Add(new DeletionError
                    {
                        Request = request,
                        ErrorMessage = ex.Message
                    });
                }
            }

            result.EndTime = DateTime.Now;
            return result;
        }

        #endregion

        #region Méthodes d'historique

        /// <summary>
        /// Enregistre une analyse dans l'historique
        /// </summary>
        public async Task<bool> SaveAnalysisToHistory(DuplicateAnalysisResult analysis)
        {
            // Si tu as une table d'historique, sinon enregistrer dans un fichier JSON
            string connStr = _connectionString39C; // Utiliser une des deux bases pour l'historique

            string query = @"
                INSERT INTO dbo.historique_analyses 
                (date_analyse, total_employes_39c, total_employes_19m, 
                 nb_codes_differents, nb_numeros_differents, nb_cas_ambigus)
                VALUES 
                (@dateAnalyse, @total39C, @total19M, 
                 @nbCodesDiff, @nbNumDiff, @nbAmbigus)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateAnalyse", analysis.AnalysisDate);
                        cmd.Parameters.AddWithValue("@total39C", analysis.TotalEmployees39C);
                        cmd.Parameters.AddWithValue("@total19M", analysis.TotalEmployees19M);
                        cmd.Parameters.AddWithValue("@nbCodesDiff", analysis.DifferentCodeDuplicates.Count);
                        cmd.Parameters.AddWithValue("@nbNumDiff", analysis.DifferentNumberDuplicates.Count);
                        cmd.Parameters.AddWithValue("@nbAmbigus", analysis.AmbiguousCases.Count);

                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch
            {
                // Si la table n'existe pas, ignorer (ou utiliser un fichier JSON)
                return false;
            }
        }

        #endregion
    }

    #region Classes de données

    public class DbEmployee
    {
        public string SiteCode { get; set; }
        public string ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CardholderIdNumber { get; set; }
        public DateTime FromDateValid { get; set; }
        public int Status { get; set; }
        public string DepartmentUID { get; set; }
        public DateTime LastDownloadTime { get; set; }
        public string AD_Username { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class SiteStatistics
    {
        public int Count39C { get; set; }
        public int Count19M { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class DuplicateAnalysisResult
    {
        public DateTime AnalysisDate { get; set; }
        public int TotalEmployees39C { get; set; }
        public int TotalEmployees19M { get; set; }
        public List<DuplicatePair> DifferentCodeDuplicates { get; set; } = new List<DuplicatePair>();
        public List<DuplicatePair> DifferentNumberDuplicates { get; set; } = new List<DuplicatePair>();
        public List<DuplicatePair> AmbiguousCases { get; set; } = new List<DuplicatePair>();

        public int TotalDuplicates => 
            DifferentCodeDuplicates.Count + 
            DifferentNumberDuplicates.Count + 
            AmbiguousCases.Count;
    }

    public class DuplicatePair
    {
        public DbEmployee Employee39C { get; set; }
        public DbEmployee Employee19M { get; set; }
        public DbDuplicateType DuplicateType { get; set; }
        public double Confidence { get; set; }
        public DateTime DetectionDate { get; set; } = DateTime.Now;
    }

    public enum DbDuplicateType
    {
        DifferentCode,      // Codes différents
        DifferentNumber,    // Numéros différents
        Ambiguous           // Cas ambigu
    }

    public class DeletionRequest
    {
        public string SiteCode { get; set; }
        public string ID { get; set; }
        public string FullName { get; set; }
    }

    public class BatchDeleteResult
    {
        public int TotalRequests { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public List<DeletionRequest> SuccessfulDeletions { get; set; } = new List<DeletionRequest>();
        public List<DeletionError> FailedDeletions { get; set; } = new List<DeletionError>();
    }

    public class DeletionError
    {
        public DeletionRequest Request { get; set; }
        public string ErrorMessage { get; set; }
    }

    #endregion
}


