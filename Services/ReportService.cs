using System;
using System.IO;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    public class ReportService
    {
        public void GeneratePdfReport(AnalysisResult result, string filePath)
        {
            // Note: Since we don't have iTextSharp or PdfSharp installed in the environment yet,
            // we will create a mock CSV/Text report instead.
            // In a real scenario, use NuGet packages.
            
            var lines = new System.Collections.Generic.List<string>();
            lines.Add($"Rapport d'analyse du {result.AnalysisDate}");
            lines.Add("------------------------------------------------");
            lines.Add($"Doublons trouvés: {result.Duplicates.Count}");
            lines.Add($"Site 39C: {result.Count39C} employés");
            lines.Add($"Site 19M: {result.Count19M} employés");
            lines.Add("");
            lines.Add("Détails:");
            
            foreach (var dup in result.Duplicates)
            {
                lines.Add($"Type: {dup.Type} | Statut: {dup.Status}");
                lines.Add($"  39C: {dup.SourceEmployee.FullName} ({dup.SourceEmployee.Code})");
                lines.Add($"  19M: {dup.DestEmployee.FullName} ({dup.DestEmployee.Code})");
                lines.Add("");
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
