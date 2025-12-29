using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Vaccine
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int RequiredAgeMonths { get; set; } // Age in months when vaccine is required

    [MaxLength(200)]
    public string? DiseasesPrevented { get; set; }

    public bool IsMandatory { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
}
