using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Attendance
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StudentId { get; set; }
    public Student? Student { get; set; }

    public Guid ClassId { get; set; }
    public Class? Class { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty; // Present, Absent, Late, Sick, Excused

    // Track if this is an excused absence (nghỉ có phép - entitled to meal refund)
    public bool IsExcusedAbsence { get; set; } = false;

    // Daily meal fee to be refunded (if excused)
    public decimal DailyMealRefund { get; set; } = 0;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
