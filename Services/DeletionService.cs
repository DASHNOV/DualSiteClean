using System;
using System.Threading.Tasks;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    public class DeletionService
    {
        private readonly DatabaseService _dbService;

        public DeletionService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> DeleteDuplicate(Duplicate duplicate, string siteToKeep)
        {
            try
            {
                // Logic: 
                // If siteToKeep is "39C", we delete from "19M" (DestEmployee usually)
                // Need to be careful about which employee object corresponds to which site in the Duplicate model.
                // In AnalysisService, Source is 39C, Dest is 19M.
                
                if (siteToKeep == "39C")
                {
                    // Delete 19M employee
                    await _dbService.DeleteEmployee(duplicate.DestEmployee.ID, "19M");
                }
                else if (siteToKeep == "19M")
                {
                    // Delete 39C employee
                    await _dbService.DeleteEmployee(duplicate.SourceEmployee.ID, "39C");
                }

                duplicate.Status = "Traité - Supprimé";
                duplicate.IsValidated = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting duplicate: {ex.Message}");
                return false;
            }
        }
    }
}
