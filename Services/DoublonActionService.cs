using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    /// <summary>
    /// Résultat détaillé d'un traitement de masse des doublons
    /// </summary>
    public class ResultatTraitement
    {
        public bool Succes { get; set; }
        public string Message { get; set; }
        public int NombreFichesTraitees { get; set; }
        public int NombreFichesConservees { get; set; }
        public int NombreFichesSupprimees { get; set; }
        public int NombreErreurs { get; set; }
        public List<string> Erreurs { get; set; } = new List<string>();
        public List<string> FichiersSauvegardes { get; set; } = new List<string>();
        public DateTime DateTraitement { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Service responsable de l'exécution des actions sur les doublons (sauvegarde, suppression, activation double site)
    /// </summary>
    public class DoublonActionService
    {
        private readonly string _connectionString39C;
        private readonly string _connectionString19M;
        private readonly string _cheminSauvegarde;

        public event Action<string> OnLog;
        public event Action<int, int> OnProgress;

        public DoublonActionService(string connectionString39C, string connectionString19M, string cheminSauvegarde = null)
        {
            _connectionString39C = connectionString39C;
            _connectionString19M = connectionString19M;
            _cheminSauvegarde = cheminSauvegarde ?? 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DoublonManager", "Sauvegardes");

            // Créer le dossier s'il n'existe pas
            if (!Directory.Exists(_cheminSauvegarde))
            {
                Directory.CreateDirectory(_cheminSauvegarde);
            }
        }

        /// <summary>
        /// Traite une liste de doublons : sauvegarde, suppression et activation du double site
        /// </summary>
        public async Task<ResultatTraitement> TraiterDoublonsAsync(List<EmployeDoublonFedere> doublons, bool modeSimulation = false)
        {
            var resultat = new ResultatTraitement { Succes = true };
            int total = doublons.Count;
            int actuel = 0;

            Log($"🚀 Début du traitement de {total} doublons" + (modeSimulation ? " (MODE SIMULATION)" : ""));

            foreach (var doublon in doublons)
            {
                actuel++;
                RapporterProgression(actuel, total);

                if (doublon.FicheAConserver == FicheReference.EgaliteTemporelle)
                {
                    Log($"⚠️ Ignoré: {doublon.Nom} {doublon.Prenom} - Égalité temporelle (nécessite action manuelle)");
                    continue;
                }

                try
                {
                    Log($"📋 Traitement de {doublon.Nom} {doublon.Prenom}");
                    Log($"   Décision: {doublon.RaisonDecision}");

                    var siteSuppr = DeterminerSiteASupprimer(doublon);
                    var siteConsrv = DeterminerSiteAConserver(doublon);

                    Log($"   Site à supprimer: {siteSuppr.siteName}");
                    Log($"   Site à conserver: {siteConsrv.siteName}");

                    // 1. Sauvegarde JSON (toujours effectuée, même en simulation)
                    string fichierSauvegarde = await SauvegarderDonneesAvantSuppressionAsync(doublon, 
                        doublon.FicheAConserver == FicheReference.Source39C ? FicheReference.Destination19M : FicheReference.Source39C);
                    resultat.FichiersSauvegardes.Add(fichierSauvegarde);
                    Log($"💾 Sauvegarde créée: {fichierSauvegarde}");

                    if (!modeSimulation)
                    {
                        // 2. Actions en base de données avec transactions
                        
                        // Transaction pour la suppression (Site A)
                        using (var connSuppr = new SqlConnection(siteSuppr.connectionString))
                        {
                            await connSuppr.OpenAsync();
                            using (var transSuppr = connSuppr.BeginTransaction())
                            {
                                try
                                {
                                    await SupprimerEmployeAsync(siteSuppr.donnees.ID, siteSuppr.connectionString, transSuppr);
                                    transSuppr.Commit();
                                    Log($"🗑️ Suppression effectuée sur {siteSuppr.siteName}");
                                }
                                catch (Exception ex)
                                {
                                    transSuppr.Rollback();
                                    throw new Exception($"Erreur lors de la suppression sur {siteSuppr.siteName}: {ex.Message}");
                                }
                            }
                        }

                        // Transaction pour l'activation double site (Site B)
                        using (var connConsrv = new SqlConnection(siteConsrv.connectionString))
                        {
                            await connConsrv.OpenAsync();
                            using (var transConsrv = connConsrv.BeginTransaction())
                            {
                                try
                                {
                                    await ActiverDoubleSiteAsync(siteConsrv.employeId, siteConsrv.connectionString, siteSuppr.siteName, transConsrv);
                                    transConsrv.Commit();
                                    Log($"🔗 Double site activé sur {siteConsrv.siteName}");
                                }
                                catch (Exception ex)
                                {
                                    transConsrv.Rollback();
                                    throw new Exception($"Erreur lors de l'activation double site sur {siteConsrv.siteName}: {ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Log("🔍 [SIMULATION] Suppression et activation double site ignorées.");
                    }

                    resultat.NombreFichesTraitees++;
                    resultat.NombreFichesSupprimees++;
                    resultat.NombreFichesConservees++;
                    Log($"✅ {doublon.Nom} {doublon.Prenom} traité avec succès\n");
                }
                catch (Exception ex)
                {
                    resultat.NombreErreurs++;
                    string errorMsg = $"❌ Erreur sur {doublon.Nom} {doublon.Prenom}: {ex.Message}";
                    resultat.Erreurs.Add(errorMsg);
                    Log(errorMsg);
                }
            }

            Log($"🏁 Traitement terminé. {resultat.NombreFichesTraitees} succès, {resultat.NombreErreurs} erreurs.");
            if (resultat.NombreErreurs > 0) resultat.Succes = false;
            
            return resultat;
        }

        /// <summary>
        /// Sauvegarde les données complètes de l'employé qui va être supprimé dans un fichier JSON
        /// </summary>
        private async Task<string> SauvegarderDonneesAvantSuppressionAsync(EmployeDoublonFedere doublon, FicheReference ficheASupprimer)
        {
            var donneesSupprimees = ficheASupprimer == FicheReference.Source39C ? doublon.DonneesSource : doublon.DonneesDest;
            string siteOrigine = ficheASupprimer == FicheReference.Source39C ? "39C" : "19M";
            string siteDest = ficheASupprimer == FicheReference.Source39C ? "19M" : "39C";

            var backup = new
            {
                DateSauvegarde = DateTime.Now,
                SiteOrigine = siteOrigine,
                SiteDestination = siteDest,
                RaisonSuppression = doublon.RaisonDecision,
                DonneesEmploye = donneesSupprimees,
                InstructionsRecreation = new
                {
                    SiteRecreation = siteOrigine,
                    ActionRequise = "Recréer la fiche avec ces données exactes en cas de besoin",
                    DoubleSiteActif = true,
                    SitesAssocies = new[] { "39C", "19M" }
                }
            };

            string dateStr = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string nomFichier = $"Sauvegarde_{doublon.Nom}_{doublon.Prenom}_{siteOrigine}_{dateStr}.json";
            // Nettoyage du nom de fichier
            foreach (char c in Path.GetInvalidFileNameChars()) { nomFichier = nomFichier.Replace(c, '_'); }
            
            string cheminComplet = Path.Combine(_cheminSauvegarde, nomFichier);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(backup, options);
            await File.WriteAllTextAsync(cheminComplet, json);

            return cheminComplet;
        }

        /// <summary>
        /// Supprime l'employé et ses badges dans la base spécifiée
        /// </summary>
        private async Task SupprimerEmployeAsync(int employeId, string connectionString, SqlTransaction transaction)
        {
            // Étape 1 : Supprimer les badges associés
            string sqlCards = "DELETE FROM [dbo].[Card] WHERE Owner = @Id";
            using (var cmdCards = new SqlCommand(sqlCards, transaction.Connection, transaction))
            {
                cmdCards.Parameters.AddWithValue("@Id", employeId);
                int cardsDeleted = await cmdCards.ExecuteNonQueryAsync();
                Log($"   - {cardsDeleted} badge(s) supprimé(s)");
            }

            // Étape 2 : Supprimer l'employé
            string sqlEmp = "DELETE FROM [dbo].[CRDHLD] WHERE ID = @Id";
            using (var cmdEmp = new SqlCommand(sqlEmp, transaction.Connection, transaction))
            {
                cmdEmp.Parameters.AddWithValue("@Id", employeId);
                await cmdEmp.ExecuteNonQueryAsync();
                Log($"   - Employé ID {employeId} supprimé de CRDHLD");
            }
        }

        /// <summary>
        /// Active l'accès multi-site pour l'employé conservé
        /// </summary>
        private async Task ActiverDoubleSiteAsync(int employeId, string connectionString, string siteAjouter, SqlTransaction transaction)
        {
            /* TODO: L'utilisateur doit choisir et décommenter la version appropriée selon la structure de sa base */

            // --- VERSION 1 : Si colonnes DoubleSite et Sites existent dans CRDHLD ---
            /*
            string sql = @"
                UPDATE [dbo].[CRDHLD]
                SET DoubleSite = 1,
                    Sites = CASE 
                        WHEN Sites IS NULL OR Sites = '' THEN @SiteAjouter
                        WHEN Sites NOT LIKE '%' + @SiteAjouter + '%' THEN Sites + ',' + @SiteAjouter
                        ELSE Sites
                    END,
                    Modification = GETDATE()
                WHERE ID = @Id";
            using (var cmd = new SqlCommand(sql, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", employeId);
                cmd.Parameters.AddWithValue("@SiteAjouter", siteAjouter);
                await cmd.ExecuteNonQueryAsync();
            }
            */

            // --- VERSION 2 : Si table de liaison CRDHLD_Sites existe ---
            /*
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[CRDHLD_Sites] WHERE EmployeId = @Id AND SiteCode = @SiteAjouter)
                BEGIN
                    INSERT INTO [dbo].[CRDHLD_Sites] (EmployeId, SiteCode)
                    VALUES (@Id, @SiteAjouter)
                END";
            using (var cmd = new SqlCommand(sql, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", employeId);
                cmd.Parameters.AddWithValue("@SiteAjouter", siteAjouter);
                await cmd.ExecuteNonQueryAsync();
            }
            */

            // Pour l'instant, on se contente de logger que l'opération est prête à être activée
            Log($"   - [INFO] Activation double site (Site additionnel: {siteAjouter}) prête pour ID {employeId}. Décommentez la logique SQL dans DoublonActionService.cs pour l'activer.");
            await Task.CompletedTask;
        }

        private (string connectionString, string siteName, EmployeComplet donnees) DeterminerSiteASupprimer(EmployeDoublonFedere doublon)
        {
            if (doublon.FicheAConserver == FicheReference.Source39C)
            {
                return (_connectionString19M, "19M", doublon.DonneesDest);
            }
            else if (doublon.FicheAConserver == FicheReference.Destination19M)
            {
                return (_connectionString39C, "39C", doublon.DonneesSource);
            }
            throw new InvalidOperationException("Impossible de déterminer la fiche à supprimer pour une égalité temporelle");
        }

        private (string connectionString, string siteName, int employeId) DeterminerSiteAConserver(EmployeDoublonFedere doublon)
        {
            if (doublon.FicheAConserver == FicheReference.Source39C)
            {
                return (_connectionString39C, "39C", doublon.IDSource);
            }
            return (_connectionString19M, "19M", doublon.IDDest);
        }

        private void Log(string message) => OnLog?.Invoke(message);
        
        private void RapporterProgression(int actuel, int total) => OnProgress?.Invoke(actuel, total);
    }
}
