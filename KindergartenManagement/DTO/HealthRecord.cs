using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class HealthRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StudentId { get; set; }
    public Student? Student { get; set; }

    [Required]
    public DateTime RecordDate { get; set; }

    public decimal? Height { get; set; } // cm
    public decimal? Weight { get; set; } // kg

    [MaxLength(500)]
    public string? VaccinationInfo { get; set; }

    [MaxLength(1000)]
    public string? MedicalConditions { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
