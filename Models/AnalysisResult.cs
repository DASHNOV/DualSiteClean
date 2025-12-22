using System;
using System.Collections.Generic;
using DoublonManager.Models;

namespace DoublonManager.Models
{
    public class AnalysisResult
    {
        public DateTime AnalysisDate { get; set; }
        public int Count39C { get; set; }
        public int Count19M { get; set; }
        public List<Duplicate> Duplicates { get; set; } = new List<Duplicate>();
    }
}
