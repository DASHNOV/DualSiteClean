using System;

namespace DoublonManager.Models
{
    public class Employee
    {
        public string SiteCode { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string CardholderIdNumber { get; set; } = string.Empty;
        public DateTime? FromDateValid { get; set; }
        public int Status { get; set; }
        public string DepartmentUID { get; set; } = string.Empty;
        public DateTime? LastDownloadTime { get; set; }
        public string AD_Username { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
    }
}
