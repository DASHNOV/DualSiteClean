using System;

namespace DoublonManager.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Num { get; set; }
        public DateTime? LocDate { get; set; }
        public string Site { get; set; } = string.Empty; // "39C" ou "19M"

        public string FullName => $"{LastName} {FirstName}";
    }
}
