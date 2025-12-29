using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Student
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(1000)]
    public string? MedicalNotes { get; set; }

    public Guid ParentId { get; set; }
    public Parent? Parent { get; set; }

    public Guid? ClassId { get; set; }
    public Class? Class { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
    public ICollection<MealTicket> MealTickets { get; set; } = new List<MealTicket>();
}
