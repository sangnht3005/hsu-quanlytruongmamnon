using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Food
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; } // Protein, Vegetable, Fruit, Grain, Dairy

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal? Calories { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
