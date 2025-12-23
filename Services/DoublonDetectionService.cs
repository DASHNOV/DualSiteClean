using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    /// <summary>
    /// Service de détection et d'analyse des doublons d'employés entre les sites
    /// </summary>
    public class DoublonDetectionService
    {
        private readonly string _connectionString39C;
        private readonly string _connectionString19M;

        /// <summary>
        /// Événement pour le logging des étapes du service
        /// </summary>
        public event Action<string> OnLog;

        public DoublonDetectionService(string connectionString39C, string connectionString19M)
        {
            _connectionString39C = connectionString39C;
            _connectionString19M = connectionString19M;
        }

        /// <summary>
        /// Exécute la détection des doublons et enrichit les résultats avec les données complètes
        /// </summary>
        /// <returns>Liste des doublons détectés et analysés</returns>
        public async Task<List<EmployeDoublonFedere>> DetecterDoublonsAsync()
        {
            Log("🔍 Début de la détection des doublons...");
            var resultats = new List<EmployeDoublonFedere>();

            try
            {
                DataTable dtDoublons = await ExecuterRequeteDetectionAsync();
                Log($"✅ {dtDoublons.Rows.Count} doublons potentiels détectés par la requête SQL.");

                foreach (DataRow row in dtDoublons.Rows)
                {
                    var doublon = new EmployeDoublonFedere
                    {
                        Nom = row["Nom"].ToString(),
                        Prenom = row["Prenom"].ToString(),
                        CodeSource = row["CodeSource"].ToString(),
                        CodeDest = row["CodeDest"].ToString(),
                        NumSource = Convert.ToInt32(row["NumSource"]),
                        NumDest = Convert.ToInt32(row["NumDest"]),
                        IDSource = Convert.ToInt32(row["IDSource"]),
                        IDDest = Convert.ToInt32(row["IDDest"]),
                        DateSource = row["DateSource"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DateSource"]),
                        DateDest = row["DateDest"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DateDest"]),
                        Statut = row["Statut"].ToString()
                    };

                    // Définir le type de doublon pour l'énum
                    if (doublon.Statut == "Code différent") doublon.TypeDoublon = TypeDoublon.CodeDifferent;
                    else if (doublon.Statut == "Numéro différent") doublon.TypeDoublon = TypeDoublon.NumeroDifferent;
                    else doublon.TypeDoublon = TypeDoublon.AutreDifference;

                    // Charger les données complètes pour analyse fine
                    doublon.DonneesSource = await ChargerDonneesCompletesAsync(doublon.IDSource, _connectionString39C, "39C");
                    doublon.DonneesDest = await ChargerDonneesCompletesAsync(doublon.IDDest, _connectionString19M, "19M");

                    // Reporter les informations de complétude dans l'objet principal
                    if (doublon.DonneesSource != null)
                    {
                        doublon.HasPhotoSource = doublon.DonneesSource.HasPhoto;
                        doublon.CompletudeSource = doublon.DonneesSource.CalculerScoreCompletude();
                    }

                    if (doublon.DonneesDest != null)
                    {
                        doublon.HasPhotoDest = doublon.DonneesDest.HasPhoto;
                        doublon.CompletudeDest = doublon.DonneesDest.CalculerScoreCompletude();
                    }

                    // Appliquer la logique de décision
                    DeterminerFicheAConserver(doublon);

                    resultats.Add(doublon);
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Erreur lors de la détection: {ex.Message}");
            }

            return resultats;
        }

        /// <summary>
        /// Exécute la requête SQL complexe de détection des doublons
        /// </summary>
        private async Task<DataTable> ExecuterRequeteDetectionAsync()
        {
            string sql = @"
WITH SourceData AS (
    SELECT
        s.Last_Name,
        s.First_Name,
        CodeSource = sc.Code,
        s.Num AS NumSource,
        s.ID AS IDSource,
        s.Loc_Date AS DateSource,
        RowNum = ROW_NUMBER() OVER (
            PARTITION BY s.Last_Name, s.First_Name
            ORDER BY sc.ID DESC
        )
    FROM [Amadeus5_39C].[dbo].[CRDHLD] s
    INNER JOIN [Amadeus5_39C].[dbo].[Card] sc
        ON s.ID = sc.Owner
    WHERE sc.Status = 1 AND sc.techno = 3
),
DestData AS (
    SELECT
        d.Last_Name,
        d.First_Name,
        CodeDest = dc.Code,
        d.Num AS NumDest,
        d.ID AS IDDest,
        d.Loc_Date AS DateDest,
        RowNum = ROW_NUMBER() OVER (
            PARTITION BY d.Last_Name, d.First_Name
            ORDER BY dc.ID DESC
        )
    FROM [Amadeus5_19M].[dbo].[CRDHLD] d
    INNER JOIN [Amadeus5_19M].[dbo].[Card] dc
        ON d.ID = dc.Owner
    WHERE dc.Status = 1 AND dc.techno = 3
)
SELECT
    s.Last_Name AS Nom,
    s.First_Name AS Prenom,
    s.CodeSource,
    d.CodeDest,
    s.NumSource,
    d.NumDest,
    s.IDSource,
    d.IDDest,
    s.DateSource,
    d.DateDest,
    Statut = CASE
        WHEN s.CodeSource <> d.CodeDest THEN 'Code différent'
        WHEN s.NumSource <> d.NumDest THEN 'Numéro différent'
        ELSE 'Autre différence'
    END
FROM SourceData s
INNER JOIN DestData d
    ON s.Last_Name = d.Last_Name
    AND s.First_Name = d.First_Name
    AND s.RowNum = 1
    AND d.RowNum = 1
WHERE
    s.CodeSource <> d.CodeDest
    OR s.NumSource <> d.NumDest
ORDER BY
    CASE
        WHEN s.CodeSource <> d.CodeDest THEN 0
        WHEN s.NumSource <> d.NumDest THEN 1
        ELSE 2
    END,
    s.Last_Name,
    s.First_Name;";

            var dt = new DataTable();
            using (var connection = new SqlConnection(_connectionString39C))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Charge toutes les données d'un employé spécifique
        /// </summary>
        private async Task<EmployeComplet> ChargerDonneesCompletesAsync(int employeId, string connectionString, string siteName)
        {
            string sql = @"
SELECT 
    -- Identification
    e.ID,
    e.Last_Name,
    e.First_Name,
    e.Num,
    
    -- Badge (jointure avec Card)
    c.Code AS CodeBadge,
    c.ID AS CardID,
    c.techno,
    
    -- Informations personnelles
    e.Email1 AS Email,
    e.Phone1 AS Telephone,
    e.Mobile,
    e.Addr1 AS Adresse,
    e.ZipCode AS CodePostal,
    e.City AS Ville,
    e.BirthDate AS DateNaissance,
    e.BirthPlace AS LieuNaissance,
    
    -- Informations professionnelles
    e.Function AS Fonction,
    e.Service,
    e.Company AS Societe,
    e.HireDate AS DateEmbauche,
    e.Badge AS Matricule,
    
    -- Photo
    e.Picture AS Photo,
    
    -- Métadonnées
    e.Creation AS DateCreation,
    e.Modification AS DateModification,
    e.Loc_Date
    
FROM [dbo].[CRDHLD] e
LEFT JOIN [dbo].[Card] c ON e.ID = c.Owner AND c.Status = 1 AND c.techno = 3
WHERE e.ID = @EmployeId";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeId", employeId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var emp = new EmployeComplet
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Nom = reader["Last_Name"]?.ToString(),
                                    Prenom = reader["First_Name"]?.ToString(),
                                    Num = reader["Num"]?.ToString(),
                                    CodeBadge = reader["CodeBadge"]?.ToString(),
                                    CardID = reader["CardID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CardID"]),
                                    Techno = reader["techno"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["techno"]),
                                    Email = reader["Email"]?.ToString(),
                                    Telephone = reader["Telephone"]?.ToString(),
                                    Mobile = reader["Mobile"]?.ToString(),
                                    Adresse = reader["Adresse"]?.ToString(),
                                    CodePostal = reader["CodePostal"]?.ToString(),
                                    Ville = reader["Ville"]?.ToString(),
                                    DateNaissance = reader["DateNaissance"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateNaissance"]),
                                    LieuNaissance = reader["LieuNaissance"]?.ToString(),
                                    Fonction = reader["Fonction"]?.ToString(),
                                    Service = reader["Service"]?.ToString(),
                                    Societe = reader["Societe"]?.ToString(),
                                    DateEmbauche = reader["DateEmbauche"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateEmbauche"]),
                                    Matricule = reader["Matricule"]?.ToString(),
                                    Photo = reader["Photo"] == DBNull.Value ? null : (byte[])reader["Photo"],
                                    DateCreation = reader["DateCreation"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateCreation"]),
                                    DateModification = reader["DateModification"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateModification"]),
                                    Loc_Date = reader["Loc_Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Loc_Date"]),
                                    SitePrincipal = siteName
                                };
                                return emp;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Erreur lors du chargement des données complets (ID:{employeId}) sur {siteName}: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Détermine quelle fiche conserver en suivant la hiérarchie : Photo > Complétude > Date
        /// </summary>
        private void DeterminerFicheAConserver(EmployeDoublonFedere doublon)
        {
            // 1. PRIORITÉ PHOTO
            if (doublon.HasPhotoSource && !doublon.HasPhotoDest)
            {
                doublon.FicheAConserver = FicheReference.Source39C;
                doublon.RaisonDecision = "📷 Photo présente sur Site 39C uniquement";
                return;
            }
            if (!doublon.HasPhotoSource && doublon.HasPhotoDest)
            {
                doublon.FicheAConserver = FicheReference.Destination19M;
                doublon.RaisonDecision = "📷 Photo présente sur Site 19M uniquement";
                return;
            }

            // 2. PRIORITÉ COMPLÉTUDE (si les deux ont une photo ou si aucun n'en a)
            if (doublon.CompletudeSource > doublon.CompletudeDest)
            {
                doublon.FicheAConserver = FicheReference.Source39C;
                doublon.RaisonDecision = $"📊 Complétude supérieure: 39C ({doublon.CompletudeSource}%) > 19M ({doublon.CompletudeDest}%)";
                return;
            }
            if (doublon.CompletudeDest > doublon.CompletudeSource)
            {
                doublon.FicheAConserver = FicheReference.Destination19M;
                doublon.RaisonDecision = $"📊 Complétude supérieure: 19M ({doublon.CompletudeDest}%) > 39C ({doublon.CompletudeSource}%)";
                return;
            }

            // 3. PRIORITÉ DATE (si photo et complétude égales)
            DateTime ds = doublon.DateSource ?? DateTime.MinValue;
            DateTime dd = doublon.DateDest ?? DateTime.MinValue;

            if (ds > dd)
            {
                doublon.FicheAConserver = FicheReference.Source39C;
                doublon.RaisonDecision = $"📅 Date plus récente sur 39C ({ds:dd/MM/yyyy})";
                return;
            }
            if (dd > ds)
            {
                doublon.FicheAConserver = FicheReference.Destination19M;
                doublon.RaisonDecision = $"📅 Date plus récente sur 19M ({dd:dd/MM/yyyy})";
                return;
            }

            // 4. ÉGALITÉ PARFAITE
            doublon.FicheAConserver = FicheReference.EgaliteTemporelle;
            doublon.RaisonDecision = "⚖️ Égalité parfaite - Intervention manuelle requise";
        }

        private void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}
