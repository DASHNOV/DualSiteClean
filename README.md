# 🔍 DoublonManager - Gestionnaire de Doublons Multi-Sites

Application Windows Forms pour la détection et la gestion des employés en double entre deux sites (39C et 19M).

![Version](https://img.shields.io/badge/version-1.0-blue)
![.NET](https://img.shields.io/badge/.NET-6.0+-purple)
![Status](https://img.shields.io/badge/status-En%20développement-orange)

## 📋 Description

**DoublonManager** est une application de bureau développée en C# avec Windows Forms qui permet de :
- 🔍 Détecter automatiquement les doublons d'employés entre deux bases de données SQL Server
- 📊 Analyser et catégoriser les doublons selon différents critères
- 🗑️ Supprimer les doublons après validation
- 📈 Exporter les résultats vers Excel
- 📜 Consulter l'historique des analyses

## ✨ Fonctionnalités principales

### 🎯 Détection intelligente des doublons

L'application détecte 3 types de doublons :

1. **⚠️ Codes différents** : Même nom, prénom et numéro mais codes employés différents
2. **⚠️ Numéros différents** : Même nom, prénom et code mais numéros d'employés différents
3. **❓ Cas ambigus** : Même nom et prénom mais codes ET numéros différents avec dates de modification identiques

### 📊 Calcul de confiance

Chaque doublon détecté se voit attribuer un score de confiance basé sur :
- Nom et prénom identiques (+40%)
- Code employé identique (+20%)
- Numéro employé identique (+20%)
- Département identique (+10%)
- Date d'embauche proche ±30 jours (+10%)

### 🎨 Interface moderne

- Dashboard avec statistiques en temps réel
- Analyse avec progression visuelle
- Résultats organisés en onglets
- Codage couleur par niveau de confiance
- Design moderne et intuitif

## 🚀 Installation

### Prérequis

- Windows 10 ou supérieur
- .NET 6.0 Runtime ou supérieur
- SQL Server 2016 ou supérieur
- Accès aux bases de données des sites 39C et 19M

### Installation

1. Téléchargez la dernière version depuis les releases
2. Extrayez l'archive dans un dossier de votre choix
3. Lancez `DoublonManager.exe`

### Première utilisation

1. Au premier lancement, configurez les connexions aux bases de données :
   - Menu **⚙️ Paramètres**
   - Configurez le **Site 39C** et le **Site 19M**
   - Testez les connexions
   - Sauvegardez

2. Retournez au dashboard et cliquez sur **🔍 Lancer une analyse**

## 📖 Guide d'utilisation

### Configuration des connexions

#### Site 39C
- **Serveur** : Nom ou IP du serveur SQL
- **Base de données** : Nom de la base de données
- **Authentification** :
  - Windows (recommandé)
  - SQL Server (utilisateur/mot de passe)

#### Site 19M
- Même configuration que le Site 39C

### Lancement d'une analyse

1. Depuis le dashboard, cliquez sur **🔍 Lancer une analyse**
2. L'analyse démarre automatiquement
3. Suivez la progression en temps réel
4. Les résultats s'affichent automatiquement

### Interprétation des résultats

#### Résumé visuel

5 cartes affichent :
- 📊 **Fiches Site 39C** : Nombre total d'employés actifs
- 📊 **Fiches Site 19M** : Nombre total d'employés actifs
- ⚠️ **Codes différents** : Nombre de doublons détectés
- ⚠️ **Numéros différents** : Nombre de doublons détectés
- ❓ **Cas ambigus** : Nombre de cas nécessitant validation

#### Codage couleur

Les lignes sont colorées selon la confiance :
- 🟢 **Vert** : Confiance ≥ 80% (très probable)
- 🟡 **Jaune** : Confiance 60-79% (probable)
- 🔴 **Rouge** : Confiance < 60% (incertain)

### Actions disponibles

#### 📊 Exporter Excel
Exporte tous les résultats vers un fichier Excel formaté.

#### 🗑️ Supprimer sélection
Supprime les doublons sélectionnés après confirmation.

#### ❌ Fermer
Ferme l'analyse et retourne au dashboard.

## 🏗️ Architecture

### Structure du projet

```
DoublonManager/
├── Forms/              # Formulaires WinForms
│   ├── MainDashboard.cs
│   ├── SettingsForm.cs
│   └── AnalysisForm.cs
├── Services/           # Services métier
│   └── DatabaseService.cs
├── Models/             # Modèles de données
├── Helpers/            # Classes utilitaires
└── SQL/                # Scripts SQL
```

### Technologies

- **Framework** : .NET 6.0+
- **UI** : Windows Forms
- **Base de données** : SQL Server
- **Sécurité** : Chiffrement AES des mots de passe

## 🔐 Sécurité

### Connexions
- ✅ Mots de passe chiffrés (AES)
- ✅ Support Windows Authentication
- ✅ Stockage sécurisé dans le registre Windows

### Données
- ✅ Suppression logique (statut = 'Supprimé')
- ✅ Traçabilité complète (qui, quand)
- ✅ Validation avant suppression

## 📊 Schéma de base de données

### Table `employes`

```sql
CREATE TABLE dbo.employes (
    code_employe NVARCHAR(50) PRIMARY KEY,
    nom NVARCHAR(100) NOT NULL,
    prenom NVARCHAR(100) NOT NULL,
    numero_employe INT,
    date_embauche DATE,
    statut NVARCHAR(20) DEFAULT 'Actif',
    departement NVARCHAR(100),
    date_modification DATETIME DEFAULT GETDATE(),
    modifie_par NVARCHAR(100)
);
```

### Table `historique_analyses` (optionnelle)

```sql
CREATE TABLE dbo.historique_analyses (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date_analyse DATETIME NOT NULL,
    total_employes_39c INT,
    total_employes_19m INT,
    nb_codes_differents INT,
    nb_numeros_differents INT,
    nb_cas_ambigus INT
);
```

## 🛠️ Développement

### Compilation

```bash
dotnet build
```

### Exécution en mode Debug

```bash
dotnet run
```

### Compilation Release

```bash
dotnet build --configuration Release
```

## 📚 Documentation

- [📄 README_DatabaseService.md](README_DatabaseService.md) - Documentation technique du DatabaseService
- [📄 README_AnalysisForm.md](README_AnalysisForm.md) - Documentation technique de l'AnalysisForm
- [📖 GUIDE_AnalysisForm.md](GUIDE_AnalysisForm.md) - Guide utilisateur de l'AnalysisForm
- [🏗️ STRUCTURE.md](STRUCTURE.md) - Structure complète du projet
- [✅ STEP0_COMPLETED.md](STEP0_COMPLETED.md) - Récapitulatif étape 0

## 🗺️ Roadmap

### ✅ Étape 0 : AnalysisForm (Terminé)
- [x] Interface d'analyse complète
- [x] Détection des 3 types de doublons
- [x] Affichage avec codage couleur
- [x] Intégration avec MainDashboard

### ⏳ Étape 4 : DeleteConfirmationForm (À venir)
- [ ] Formulaire de confirmation
- [ ] Choix du site à conserver
- [ ] Suppression en batch
- [ ] Rapport de suppression

### ⏳ Étape 5 : Export Excel (À venir)
- [ ] Export formaté
- [ ] Graphiques et statistiques
- [ ] Ouverture automatique

### 🔮 Améliorations futures
- [ ] Graphiques de tendances
- [ ] Notifications par email
- [ ] Logs détaillés
- [ ] Synchronisation automatique
- [ ] Thèmes personnalisables
- [ ] Interface web

## 🐛 Résolution de problèmes

### Erreur de connexion

**Problème** : "Impossible de se connecter à la base de données"

**Solutions** :
1. Vérifiez le nom du serveur et de la base de données
2. Vérifiez les permissions de l'utilisateur
3. Testez la connexion dans SQL Server Management Studio
4. Vérifiez le pare-feu Windows

### Aucun doublon détecté

**Problème** : L'analyse ne trouve aucun doublon

**Solutions** :
1. Vérifiez que les deux bases contiennent des données
2. Vérifiez que le statut des employés est 'Actif'
3. Vérifiez les critères de détection dans DatabaseService.cs

### Erreur lors de la suppression

**Problème** : "Erreur lors de la suppression de l'employé"

**Solutions** :
1. Vérifiez les permissions de l'utilisateur (UPDATE)
2. Vérifiez que l'employé existe toujours
3. Consultez les logs pour plus de détails

## 📞 Support

Pour toute question ou problème :
1. Consultez la documentation
2. Vérifiez les issues GitHub
3. Contactez l'administrateur système

## 👥 Contributeurs

- **Développeur principal** : [Votre nom]
- **Date de création** : 2025-12-22

## 📄 Licence

Ce projet est propriétaire et confidentiel.

## 🙏 Remerciements

Merci à tous ceux qui ont contribué à ce projet !

---

**Version** : 1.0  
**Dernière mise à jour** : 2025-12-22  
**Statut** : En développement actif
