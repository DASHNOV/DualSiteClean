using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

            string query = @"
                SELECT COUNT(*) 
                FROM dbo.employes 
                WHERE statut = 'Actif'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
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

            string query = @"
                SELECT 
                    code_employe,
                    nom,
                    prenom,
                    numero_employe,
                    date_embauche,
                    statut,
                    departement,
                    date_modification,
                    modifie_par
                FROM dbo.employes
                WHERE statut = 'Actif'
                ORDER BY nom, prenom";

            List<DbEmployee> employees = new List<DbEmployee>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
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
                                    CodeEmploye = reader["code_employe"].ToString(),
                                    Nom = reader["nom"].ToString(),
                                    Prenom = reader["prenom"].ToString(),
                                    NumeroEmploye = reader["numero_employe"] != DBNull.Value 
                                        ? Convert.ToInt32(reader["numero_employe"]) : 0,
                                    DateEmbauche = reader["date_embauche"] != DBNull.Value 
                                        ? Convert.ToDateTime(reader["date_embauche"]) : DateTime.MinValue,
                                    Statut = reader["statut"].ToString(),
                                    Departement = reader["departement"].ToString(),
                                    DateModification = reader["date_modification"] != DBNull.Value 
                                        ? Convert.ToDateTime(reader["date_modification"]) : DateTime.MinValue,
                                    ModifiePar = reader["modifie_par"].ToString()
                                });
                            }
                        }
                    }
                }

                return employees;
            }
            catch (Exception ex)
            {
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

                // Détection par code différent
                result.DifferentCodeDuplicates = DetectDifferentCodeDuplicates(employees39C, employees19M);

                // Détection par numéro différent
                result.DifferentNumberDuplicates = DetectDifferentNumberDuplicates(employees39C, employees19M);

                // Détection des cas ambigus
                result.AmbiguousCases = DetectAmbiguousCases(employees39C, employees19M);

                return result;
            }
            catch (Exception ex)
            {
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
                    if (emp39C.Nom.Equals(emp19M.Nom, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.Prenom.Equals(emp19M.Prenom, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.NumeroEmploye == emp19M.NumeroEmploye &&
                        emp39C.NumeroEmploye != 0 &&
                        !emp39C.CodeEmploye.Equals(emp19M.CodeEmploye, StringComparison.OrdinalIgnoreCase))
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
                    if (emp39C.Nom.Equals(emp19M.Nom, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.Prenom.Equals(emp19M.Prenom, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.CodeEmploye.Equals(emp19M.CodeEmploye, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.NumeroEmploye != emp19M.NumeroEmploye &&
                        emp39C.NumeroEmploye != 0 &&
                        emp19M.NumeroEmploye != 0)
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
                    if (emp39C.Nom.Equals(emp19M.Nom, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.Prenom.Equals(emp19M.Prenom, StringComparison.OrdinalIgnoreCase) &&
                        !emp39C.CodeEmploye.Equals(emp19M.CodeEmploye, StringComparison.OrdinalIgnoreCase) &&
                        emp39C.NumeroEmploye != emp19M.NumeroEmploye)
                    {
                        // Cas ambigu si dates de modification identiques
                        if (Math.Abs((emp39C.DateModification - emp19M.DateModification).TotalHours) < 1)
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
            if (emp1.Nom.Equals(emp2.Nom, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;
            if (emp1.Prenom.Equals(emp2.Prenom, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;

            // Code identique : +20%
            if (emp1.CodeEmploye.Equals(emp2.CodeEmploye, StringComparison.OrdinalIgnoreCase))
                confidence += 0.20;

            // Numéro identique : +20%
            if (emp1.NumeroEmploye == emp2.NumeroEmploye && emp1.NumeroEmploye != 0)
                confidence += 0.20;

            // Département identique : +10%
            if (emp1.Departement.Equals(emp2.Departement, StringComparison.OrdinalIgnoreCase))
                confidence += 0.10;

            // Date d'embauche proche (±30 jours) : +10%
            if (Math.Abs((emp1.DateEmbauche - emp2.DateEmbauche).TotalDays) <= 30)
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

            // Option 1 : Suppression logique (recommandé)
            string query = @"
                UPDATE dbo.employes 
                SET 
                    statut = 'Supprimé',
                    date_modification = GETDATE(),
                    modifie_par = @deletedBy
                WHERE code_employe = @codeEmploye";

            // Option 2 : Suppression physique (décommenter si nécessaire)
            // string query = "DELETE FROM dbo.employes WHERE code_employe = @codeEmploye";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codeEmploye", codeEmploye);
                        cmd.Parameters.AddWithValue("@deletedBy", deletedBy);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
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
                    bool success = await DeleteEmployee(request.SiteCode, request.CodeEmploye, deletedBy);
                    
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
        public string CodeEmploye { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public int NumeroEmploye { get; set; }
        public DateTime DateEmbauche { get; set; }
        public string Statut { get; set; }
        public string Departement { get; set; }
        public DateTime DateModification { get; set; }
        public string ModifiePar { get; set; }

        public string NomComplet => $"{Prenom} {Nom}";
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
        public string CodeEmploye { get; set; }
        public string NomComplet { get; set; }
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


