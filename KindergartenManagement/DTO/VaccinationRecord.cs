using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class VaccinationRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid StudentId { get; set; }
    public Student? Student { get; set; }

    [Required]
    public Guid VaccineId { get; set; }
    public Vaccine? Vaccine { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Not Done"; // Done, Not Done, Scheduled

    public DateTime? VaccinationDate { get; set; }

    [MaxLength(300)]
    public string? MedicalUnit { get; set; }

    [MaxLength(100)]
    public string? LotNumber { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
