using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Grade
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int MinAge { get; set; }
    public int MaxAge { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
