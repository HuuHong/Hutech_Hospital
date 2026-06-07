using System;
using Microsoft.AspNetCore.Identity;

namespace HUTECH_Hospital.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
