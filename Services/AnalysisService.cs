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
                LastName = e.LastName,
                FirstName = e.FirstName,
                ID = e.ID,
                CardholderIdNumber = e.CardholderIdNumber,
                FromDateValid = e.FromDateValid,
                SiteCode = "39C",
                Status = e.Status,
                DepartmentUID = e.DepartmentUID,
                LastDownloadTime = e.LastDownloadTime,
                AD_Username = e.AD_Username
            }).ToList();

            var employees19M = dbEmployees19M.Select(e => new Employee
            {
                LastName = e.LastName,
                FirstName = e.FirstName,
                ID = e.ID,
                CardholderIdNumber = e.CardholderIdNumber,
                FromDateValid = e.FromDateValid,
                SiteCode = "19M",
                Status = e.Status,
                DepartmentUID = e.DepartmentUID,
                LastDownloadTime = e.LastDownloadTime,
                AD_Username = e.AD_Username
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
                    if (emp39C.ID == match.ID && emp39C.CardholderIdNumber == match.CardholderIdNumber)
                    {
                        // Exact match on identifiers
                         duplicate.Type = DuplicateType.CasAmbigu;
                         duplicate.Status = "Identique";
                    }
                    else if (emp39C.ID != match.ID)
                    {
                        duplicate.Type = DuplicateType.CodeDifferent;
                        duplicate.Status = "Différence ID";
                        duplicate.Recommendation = $"Garder {emp39C.ID} ?";
                    }
                    else if (emp39C.CardholderIdNumber != match.CardholderIdNumber)
                    {
                        duplicate.Type = DuplicateType.NumeroDifferent;
                        duplicate.Status = "Différence Numéro";
                        duplicate.Recommendation = $"Garder {emp39C.CardholderIdNumber} ?";
                    }

                    result.Duplicates.Add(duplicate);
                }
            }

            return result;
        }
    }
}
