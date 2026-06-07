namespace HUTECH_Hospital.ViewModels
{
    public class MedicineViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Usage { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
