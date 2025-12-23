using System;

namespace DoublonManager.Models
{
    /// <summary>
    /// Type de doublon détecté lors de l'analyse
    /// </summary>
    public enum TypeDoublon
    {
        /// <summary>Les codes badges diffèrent</summary>
        CodeDifferent,
        /// <summary>Les numéros badges diffèrent</summary>
        NumeroDifferent,
        /// <summary>Autre type de différence</summary>
        AutreDifference
    }

    /// <summary>
    /// Indique quelle fiche doit être conservée comme référence
    /// </summary>
    public enum FicheReference
    {
        /// <summary>Conserver la fiche du Site 39C</summary>
        Source39C,
        /// <summary>Conserver la fiche du Site 19M</summary>
        Destination19M,
        /// <summary>Égalité parfaite - Intervention manuelle requise</summary>
        EgaliteTemporelle
    }

    /// <summary>
    /// Représente un doublon détecté dans la base fédératrice
    /// </summary>
    public class EmployeDoublonFedere
    {
        // Identification de base
        public string Nom { get; set; }
        public string Prenom { get; set; }

        // Données Site Source (39C)
        public string CodeSource { get; set; }
        public int NumSource { get; set; }
        public int IDSource { get; set; }
        public DateTime? DateSource { get; set; }
        public bool HasPhotoSource { get; set; }
        public int CompletudeSource { get; set; }

        // Données Site Destination (19M)
        public string CodeDest { get; set; }
        public int NumDest { get; set; }
        public int IDDest { get; set; }
        public DateTime? DateDest { get; set; }
        public bool HasPhotoDest { get; set; }
        public int CompletudeDest { get; set; }

        // Analyse et décision
        public string Statut { get; set; } // "Code différent", "Numéro différent", "Autre différence"
        public TypeDoublon TypeDoublon { get; set; }
        public FicheReference FicheAConserver { get; set; }
        public string RaisonDecision { get; set; }

        // Données complètes pour sauvegarde
        public EmployeComplet DonneesSource { get; set; }
        public EmployeComplet DonneesDest { get; set; }
    }

    /// <summary>
    /// Contient toutes les données d'un employé pour sauvegarde/recréation
    /// </summary>
    public class EmployeComplet
    {
        // Identification
        public int ID { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Num { get; set; }

        // Badge
        public string CodeBadge { get; set; }
        public int? CardID { get; set; }
        public int? Techno { get; set; }

        // Informations personnelles
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public DateTime? DateNaissance { get; set; }
        public string LieuNaissance { get; set; }

        // Informations professionnelles
        public string Fonction { get; set; }
        public string Service { get; set; }
        public string Societe { get; set; }
        public DateTime? DateEmbauche { get; set; }
        public string Matricule { get; set; }

        // Photo
        public byte[] Photo { get; set; }

        /// <summary>
        /// Indique si l'employé possède une photo
        /// </summary>
        public bool HasPhoto => Photo != null && Photo.Length > 0;

        // Gestion des sites
        public string SitePrincipal { get; set; }
        public string SiteSecondaire { get; set; }
        public bool DoubleSiteActif { get; set; }

        // Métadonnées
        public DateTime? DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public DateTime? Loc_Date { get; set; }

        /// <summary>
        /// Calcule le score de complétude des données (0-100%)
        /// Logicielle : 20 champs importants. Chaque champ +1, Photo +2.
        /// </summary>
        /// <returns>Score entre 0 et 100</returns>
        public int CalculerScoreCompletude()
        {
            int score = 0;
            const int totalChamps = 20;

            // 1-5: Identification & Contact
            if (!string.IsNullOrWhiteSpace(Nom)) score++;
            if (!string.IsNullOrWhiteSpace(Prenom)) score++;
            if (!string.IsNullOrWhiteSpace(Email)) score++;
            if (!string.IsNullOrWhiteSpace(Telephone)) score++;
            if (!string.IsNullOrWhiteSpace(Mobile)) score++;

            // 6-8: Adresse
            if (!string.IsNullOrWhiteSpace(Adresse)) score++;
            if (!string.IsNullOrWhiteSpace(CodePostal)) score++;
            if (!string.IsNullOrWhiteSpace(Ville)) score++;

            // 9-10: Personnel
            if (DateNaissance.HasValue) score++;
            if (!string.IsNullOrWhiteSpace(LieuNaissance)) score++;

            // 11-13: Pro
            if (!string.IsNullOrWhiteSpace(Fonction)) score++;
            if (!string.IsNullOrWhiteSpace(Service)) score++;
            if (!string.IsNullOrWhiteSpace(Societe)) score++;

            // 14-15: Pro suite
            if (DateEmbauche.HasValue) score++;
            if (!string.IsNullOrWhiteSpace(Matricule)) score++;

            // 16: Photo (Compte double)
            if (HasPhoto) score += 2;

            // 17-18: Sites
            if (!string.IsNullOrWhiteSpace(SitePrincipal)) score++;
            if (!string.IsNullOrWhiteSpace(SiteSecondaire)) score++;

            // 19-20: Dates
            if (DateCreation.HasValue) score++;
            if (Loc_Date.HasValue) score++;

            // Calcul final
            int result = (score * 100) / totalChamps;
            
            // On plafonne à 100 si jamais (par exemple si score arrive à 21/20)
            return result > 100 ? 100 : result;
        }
    }
}
