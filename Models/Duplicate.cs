namespace DoublonManager.Models
{
    public enum DuplicateType
    {
        CodeDifferent,
        NumeroDifferent,
        CasAmbigu
    }

    public class Duplicate
    {
        public Employee SourceEmployee { get; set; } = new Employee();
        public Employee DestEmployee { get; set; } = new Employee();
        public DuplicateType Type { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public bool IsValidated { get; set; }
    }
}
