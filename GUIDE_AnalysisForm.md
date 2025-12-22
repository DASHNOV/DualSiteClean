# 📖 Guide d'utilisation - AnalysisForm

## Lancement de l'analyse

### Depuis le MainDashboard

1. Cliquez sur le bouton **"🔍 Lancer une analyse"**
2. Si la configuration n'est pas faite, le formulaire de paramètres s'ouvrira automatiquement
3. Une fois configuré, l'AnalysisForm s'ouvre et démarre l'analyse automatiquement

### Processus d'analyse automatique

L'analyse se déroule en 4 étapes visibles dans la barre de progression :

1. **📥 Chargement des données du site 39C...**
   - Connexion à la base de données du site 39C
   - Récupération de tous les employés actifs

2. **📥 Chargement des données du site 19M...**
   - Connexion à la base de données du site 19M
   - Récupération de tous les employés actifs

3. **🔍 Analyse des doublons en cours...**
   - Comparaison des employés entre les deux sites
   - Détection des codes différents
   - Détection des numéros différents
   - Identification des cas ambigus

4. **📊 Préparation des résultats...**
   - Organisation des résultats par catégorie
   - Calcul des niveaux de confiance
   - Affichage de l'interface

## Interprétation des résultats

### Résumé visuel (5 cartes)

#### 📊 Fiches Site 39C (Vert)
Nombre total d'employés actifs dans la base du site 39C

#### 📊 Fiches Site 19M (Bleu)
Nombre total d'employés actifs dans la base du site 19M

#### ⚠️ Codes différents (Rouge)
Employés détectés avec :
- ✅ Même nom et prénom
- ✅ Même numéro d'employé
- ❌ Codes employés différents

**Risque** : Moyen à élevé - Probable doublon avec erreur de code

#### ⚠️ Numéros différents (Orange)
Employés détectés avec :
- ✅ Même nom et prénom
- ✅ Même code employé
- ❌ Numéros d'employés différents

**Risque** : Moyen - Possible doublon avec erreur de numéro

#### ❓ Cas ambigus (Violet)
Employés détectés avec :
- ✅ Même nom et prénom
- ❌ Codes employés différents
- ❌ Numéros d'employés différents
- ⚠️ Dates de modification identiques (±1h)

**Risque** : Élevé - Nécessite validation manuelle

## Navigation dans les onglets

### Onglet "⚠️ Codes différents"

Affiche tous les doublons avec codes différents.

**Colonnes affichées :**
- ☑️ : Checkbox pour sélectionner la ligne
- **Site 39C - Code** : Code employé du site 39C
- **Site 39C - Nom** : Nom complet (Prénom Nom)
- **Site 39C - Numéro** : Numéro d'employé
- **Site 39C - Modif.** : Date de dernière modification
- **Site 19M - Code** : Code employé du site 19M
- **Site 19M - Nom** : Nom complet
- **Site 19M - Numéro** : Numéro d'employé
- **Site 19M - Modif.** : Date de dernière modification
- **Confiance** : Niveau de confiance (0-100%)

### Onglet "⚠️ Numéros différents"

Affiche tous les doublons avec numéros différents.
Même structure que l'onglet précédent.

### Onglet "❓ Cas ambigus"

Affiche les cas nécessitant une validation manuelle.

**Panneau d'aide :**
> ⚠️ Ces cas nécessitent une validation manuelle : même nom/prénom mais codes ET numéros différents avec dates de modification identiques. Risque de confusion élevé.

## Codage couleur des lignes

Les lignes sont colorées selon le niveau de confiance calculé :

### 🟢 Vert clair (Confiance ≥ 80%)
- Très forte probabilité de doublon
- Action recommandée : Vérifier et supprimer
- Exemple : Nom, prénom, numéro identiques + département identique

### 🟡 Jaune clair (Confiance 60-79%)
- Probabilité moyenne de doublon
- Action recommandée : Vérifier attentivement avant suppression
- Exemple : Nom, prénom identiques + numéro identique

### 🔴 Rouge clair (Confiance < 60%)
- Probabilité faible de doublon
- Action recommandée : Validation manuelle obligatoire
- Exemple : Nom, prénom identiques uniquement

## Calcul du niveau de confiance

Le système calcule automatiquement un score de confiance basé sur :

| Critère | Points |
|---------|--------|
| Nom identique | +20% |
| Prénom identique | +20% |
| Code employé identique | +20% |
| Numéro employé identique | +20% |
| Département identique | +10% |
| Date d'embauche proche (±30 jours) | +10% |

**Score maximum** : 100%

## Actions disponibles

### 📊 Exporter Excel

1. Cliquez sur le bouton **"📊 Exporter Excel"**
2. Choisissez l'emplacement et le nom du fichier
3. Le fichier Excel sera généré avec tous les résultats

**Format du fichier :**
- Nom : `Analyse_Doublons_YYYYMMDD_HHMMSS.xlsx`
- Contenu : Tous les doublons détectés avec détails complets

> ⚠️ **Note** : Cette fonctionnalité sera implémentée à l'étape 5

### 🗑️ Supprimer sélection

1. Cochez les lignes à supprimer dans la grille
2. Cliquez sur **"🗑️ Supprimer sélection"**
3. Un formulaire de confirmation s'ouvrira (étape 4)
4. Validez la suppression
5. Les doublons seront supprimés de la base de données

**Sélection multiple :**
- Cochez plusieurs lignes pour supprimer en lot
- Vous pouvez sélectionner des lignes dans différents onglets
- Seules les lignes de l'onglet actif seront prises en compte

> ⚠️ **Note** : Le formulaire de confirmation sera implémenté à l'étape 4

### ❌ Fermer

Ferme le formulaire d'analyse et retourne au dashboard principal.

**Effet :**
- Le dashboard sera automatiquement rafraîchi
- Les statistiques seront mises à jour

## Gestion des erreurs

### Erreur de connexion

Si une erreur de connexion survient :
1. Un message d'erreur s'affiche
2. Le formulaire se ferme automatiquement
3. Vérifiez la configuration dans les paramètres

### Erreur pendant l'analyse

Si une erreur survient pendant l'analyse :
1. La progression s'arrête
2. Un message d'erreur détaillé s'affiche
3. Le formulaire se ferme
4. Consultez les logs pour plus de détails

## Bonnes pratiques

### Avant de supprimer

1. ✅ Vérifiez le niveau de confiance
2. ✅ Comparez les dates de modification
3. ✅ Vérifiez les départements
4. ✅ Consultez les cas avec confiance < 80%
5. ✅ Faites une sauvegarde de la base de données

### Cas ambigus

Pour les cas ambigus :
1. ⚠️ Ne supprimez JAMAIS automatiquement
2. ⚠️ Vérifiez manuellement dans les bases de données
3. ⚠️ Contactez le service RH si nécessaire
4. ⚠️ Documentez votre décision

### Export Excel

1. 📊 Exportez les résultats avant toute suppression
2. 📊 Conservez l'export comme trace
3. 📊 Partagez avec le service RH pour validation

## Raccourcis clavier

| Touche | Action |
|--------|--------|
| Échap | Fermer le formulaire |
| Ctrl+E | Exporter Excel (si disponible) |
| Ctrl+D | Supprimer sélection (si disponible) |

## Fréquence d'analyse recommandée

- **Quotidienne** : Si ajouts fréquents d'employés
- **Hebdomadaire** : Pour maintenance régulière
- **Mensuelle** : Pour audit complet
- **Après migration** : Obligatoire après import de données

## Support

En cas de problème :
1. Vérifiez la configuration des connexions
2. Consultez les logs d'erreur
3. Vérifiez les permissions sur les bases de données
4. Contactez l'administrateur système
