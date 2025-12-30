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
    public string Type { get; set; } = string.Empty; // Học phí, Sửa chữa, Lương, Khác

    [Required]
    public decimal Amount { get; set; }

    // For tuition type: monthly tuition fee
    public decimal MonthlyTuitionFee { get; set; }

    // For tuition type: meal balance from previous month (if negative, deduct from current invoice)
    public decimal? MealBalanceFromPreviousMonth { get; set; }

    // Final amount after adjustments (for display)
    public decimal? FinalAmount { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Chưa thanh toán"; // Chưa thanh toán, Đã thanh toán, Quá hạn, Hủy

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }

    public Guid? ClassId { get; set; }
    public Class? Class { get; set; }

    // For salary invoices: UserId (staff member)
    // For tuition invoices: StudentId
    // For maintenance invoices: ClassId
    
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
