using System;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class HealthSurvey
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public double? Height { get; set; }
        public double? Weight { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        public string? Gender { get; set; }
        public string? BloodType { get; set; }
        
        // Medical history
        public string? PersonalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? CurrentMedication { get; set; }
        public string? FamilyHistory { get; set; }

        // Underlying conditions represented as a single comma-separated string, or individual booleans.
        // It's easier to store as a single string for simplicity (e.g. "Cao huyết áp, Tiểu đường").
        // We can manage this via the ViewModel.
        public string? UnderlyingConditions { get; set; }

        // Current status
        public string? CurrentSymptoms { get; set; }
        
        [Range(1, 10)]
        public int? DiscomfortLevel { get; set; }
        public string? SymptomDuration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
