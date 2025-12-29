using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class MealTicket
{
    [Key]
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }
    public Student? Student { get; set; }

    public Guid MenuId { get; set; }
    public Menu? Menu { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public bool IsConsumed { get; set; } = false;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
