using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class HealthRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid StudentId { get; set; }
    public Student? Student { get; set; }

    [Required]
    public int Month { get; set; } // 1-12

    [Required]
    public int Year { get; set; }

    public decimal? Height { get; set; } // cm
    public decimal? Weight { get; set; } // kg

    [MaxLength(100)]
    public string? GeneralHealth { get; set; } // Good, Fair, Poor

    [MaxLength(1000)]
    public string? MedicalConditions { get; set; }

    [MaxLength(2000)]
    public string? TeacherComments { get; set; }

    [MaxLength(2000)]
    public string? TeacherEvaluation { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
