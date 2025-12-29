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
    public int DayOfWeek { get; set; } // 0=CN, 1=T2, 2=T3, 3=T4, 4=T5, 5=T6, 6=T7

    [Required]
    [MaxLength(50)]
    public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Snack, Dinner

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<MenuDish> MenuDishes { get; set; } = new List<MenuDish>();
    public ICollection<MealTicket> MealTickets { get; set; } = new List<MealTicket>();
}
