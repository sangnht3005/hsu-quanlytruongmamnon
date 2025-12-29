using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Invoice
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // Tuition, Salary, Activity, Expense

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
