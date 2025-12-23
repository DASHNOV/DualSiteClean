# 🚀 DoublonManager - Gestionnaire de doublons Amadeus

Application Windows Forms (.NET 6+) pour gérer les employés en doublon entre deux sites Amadeus (39C et 19M) via une base de données fédératrice SQL Server.

---

## 📋 Fonctionnalités principales

### 🔍 Détection intelligente des doublons
- Analyse automatique des employés présents sur les deux sites
- Détection basée sur Nom + Prénom identiques
- Identification des différences de badges (Code ou Numéro)

### 📊 Analyse et scoring
- **Calcul du score de complétude** (0-100%) basé sur 20 champs clés
- **Détection de la présence de photos** (+2 points bonus)
- **Logique de décision automatique** :
  1. 📷 **Priorité Photo** : Conserve la fiche avec photo si l'autre n'en a pas
  2. 📈 **Priorité Complétude** : En cas d'égalité photo, conserve le meilleur score
  3. 📅 **Priorité Date** : En cas d'égalité, conserve la fiche modifiée le plus récemment
  4. ⚠️ **Égalité parfaite** : Signale les cas nécessitant une intervention manuelle

### ⚙️ Actions automatisées
- **Sauvegarde complète** avant suppression (fichiers JSON horodatés)
- **Suppression sécurisée** de l'employé et de ses badges
- **Activation du double site** sur la fiche conservée
- **Mode simulation** pour tester sans modifier les bases

### 📈 Interface utilisateur
- **DataGridView** avec formatage conditionnel (vert/jaune)
- **Panneau de détails** comparatif pour chaque doublon
- **Barre de progression** en temps réel
- **Console de logs** détaillée (style terminal)
- **Rapports** d'exécution complets

---

## 🏗️ Architecture du projet
DoublonManager/
├── Models/
│   └── EmployeDoublonFedere.cs      # Modèle principal des doublons
│
├── Services/
│   ├── DoublonDetectionService.cs    # Détection et analyse
│   └── DoublonActionService.cs       # Traitement et suppression
│
├── Forms/
│   ├── FormDoublonManager.cs         # Formulaire principal
│   └── SplashScreen.cs               # Écran de démarrage
│
├── Helpers/
│   └── ConfigManager.cs              # Gestion de la configuration
│
├── Program.cs                         # Point d'entrée
├── App.config                         # Configuration
└── README.md                          # Documentation

---

## ⚙️ Configuration

### 1️⃣ Modifier les chaînes de connexion

**Option A : Via App.config**
```xml
<appSettings>
    <add key="ConnectionString39C" value="Server=VOTRE_SERVER_39C;Database=Amadeus;Integrated Security=true;" />
    <add key="ConnectionString19M" value="Server=VOTRE_SERVER_19M;Database=Amadeus;Integrated Security=true;" />
</appSettings>
```

**Option B : Via l'interface**
Les chaînes de connexion peuvent être saisies directement dans les TextBox de l'application et seront sauvegardées automatiquement.

### 2️⃣ Configurer l'activation du double site
⚠️ ACTION REQUISE : Ouvrir `Services/DoublonActionService.cs` et décommenter la version SQL correspondant à votre structure de base Amadeus :
- **Version 1** : Colonne `DoubleSite`
- **Version 2** : Table de liaison `CRDHLD_Sites`

---

## 🚀 Utilisation

### Étape 1 : Détecter les doublons
1. Lancer l'application
2. Vérifier/modifier les chaînes de connexion
3. Cliquer sur "**Détecter les doublons**"
4. Attendre la fin de l'analyse

### Étape 2 : Analyser les résultats
Les doublons s'affichent dans la grille avec un code couleur :
- 🟢 **Vert** : Décision automatique possible
- 🟡 **Jaune** : Égalité parfaite (intervention manuelle requise)
Sélectionner un doublon pour voir les détails dans le panneau de comparaison.

### Étape 3 : Traiter les doublons

**Option A : Mode Simulation (recommandé)**
1. Cliquer sur "**Mode simulation**"
2. Vérifier les logs et les fichiers JSON générés
3. S'assurer que tout est conforme

**Option B : Traitement réel**
1. Cliquer sur "**Traiter automatiquement**"
2. Confirmer l'action (⚠️ **IRRÉVERSIBLE**)
3. Suivre la progression en temps réel
4. Consulter le rapport final

---

## 📁 Fichiers générés

### Sauvegardes JSON
- **Emplacement** : `Mes Documents\DoublonManager\Sauvegardes\`
- **Nom de fichier** : `Sauvegarde_NOM_Prenom_YYYYMMDD_HHmmss.json`

### Logs d'erreur
- **Emplacement** : `Mes Documents\DoublonManager\Logs\`
- **Nom de fichier** : `Error_YYYYMMDD.log`

---

## 📊 Statistiques de complétude
Le score de complétude est calculé sur 20 champs clés répartis en catégories (Identité, Badge, Pro, Perso, Photo).

---

## 🔒 Sécurité
- **Sauvegardes automatiques** : Créées AVANT toute suppression.
- **Transactions SQL** : Rollback automatique en cas d'erreur.
- **Mode simulation** : Validation du processus sans modification en base.

---

## 📜 Licence
© 2025 - Application interne
Version 1.0.0 - Janvier 2025
