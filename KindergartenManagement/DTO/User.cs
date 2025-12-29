using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    public decimal? Salary { get; set; }

    public decimal? Allowance { get; set; }

    [MaxLength(200)]
    public string? Specialization { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }

    public ICollection<StaffLeave> StaffLeaves { get; set; } = new List<StaffLeave>();
}
