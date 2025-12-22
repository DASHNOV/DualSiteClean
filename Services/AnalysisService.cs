using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    public class AnalysisService
    {
        private readonly DatabaseService _dbService;

        public AnalysisService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<AnalysisResult> AnalyzeDuplicates()
        {
            var result = new AnalysisResult
            {
                AnalysisDate = DateTime.Now,
                Duplicates = new List<Duplicate>()
            };

            // 1. Fetch data from new DatabaseService (returns DbEmployee)
            var dbEmployees39C = await _dbService.GetAllEmployees("39C");
            var dbEmployees19M = await _dbService.GetAllEmployees("19M");

            // 2. Convert DbEmployee to Employee model for compatibility
            var employees39C = dbEmployees39C.Select(e => new Employee
            {
                ID = 0, // Not used
                LastName = e.Nom,
                FirstName = e.Prenom,
                Code = e.CodeEmploye,
                Num = e.NumeroEmploye,
                LocDate = e.DateEmbauche,
                Site = "39C"
            }).ToList();

            var employees19M = dbEmployees19M.Select(e => new Employee
            {
                ID = 0, // Not used
                LastName = e.Nom,
                FirstName = e.Prenom,
                Code = e.CodeEmploye,
                Num = e.NumeroEmploye,
                LocDate = e.DateEmbauche,
                Site = "19M"
            }).ToList();

            result.Count39C = employees39C.Count;
            result.Count19M = employees19M.Count;

            // 3. Perform Analysis
            foreach (var emp39C in employees39C)
            {
                // Match primarily on Name (Last + First) for this specific business logic
                // Or matches can be more complex based on Code/Num
                
                // Find potential matches in 19M
                var matches = employees19M.Where(e => 
                    e.LastName.Equals(emp39C.LastName, StringComparison.OrdinalIgnoreCase) && 
                    e.FirstName.Equals(emp39C.FirstName, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var match in matches)
                {
                    var duplicate = new Duplicate
                    {
                        SourceEmployee = emp39C,
                        DestEmployee = match,
                        IsValidated = false
                    };

                    // Analyze type
                    if (emp39C.Code == match.Code && emp39C.Num == match.Num)
                    {
                        // Exact match on identifiers, maybe just different location?
                        // If logic implies they shouldn't exist in both, it's a duplicate.
                        // For this exercise, let's mark it as Ambiguous if everything is identical (why is it a duplicate?)
                        // Or maybe "Parfait doublon"
                         duplicate.Type = DuplicateType.CasAmbigu;
                         duplicate.Status = "Identique";
                    }
                    else if (emp39C.Code != match.Code)
                    {
                        duplicate.Type = DuplicateType.CodeDifferent;
                        duplicate.Status = "Différence Code";
                        duplicate.Recommendation = $"Garder {emp39C.Code} ?";
                    }
                    else if (emp39C.Num != match.Num)
                    {
                         duplicate.Type = DuplicateType.NumeroDifferent;
                         duplicate.Status = "Différence Numéro";
                         duplicate.Recommendation = $"Garder {emp39C.Num} ?";
                    }

                    result.Duplicates.Add(duplicate);
                }
            }

            return result;
        }
    }
}
