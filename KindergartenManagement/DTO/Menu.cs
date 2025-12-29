using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Menu
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(50)]
    public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Snack, Dinner

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<MenuFood> MenuFoods { get; set; } = new List<MenuFood>();
    public ICollection<MealTicket> MealTickets { get; set; } = new List<MealTicket>();
}
