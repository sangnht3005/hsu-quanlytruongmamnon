using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class StaffLeave
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string LeaveType { get; set; } = string.Empty; // Sick, Personal, Vacation, Unpaid

    [MaxLength(1000)]
    public string? Reason { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [MaxLength(1000)]
    public string? ApprovalNotes { get; set; }

    public Guid? ApprovedBy { get; set; }
    public User? Approver { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
