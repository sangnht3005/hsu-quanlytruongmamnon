using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class MenuDish
{
    public Guid MenuId { get; set; }
    public Menu? Menu { get; set; }

    public Guid DishId { get; set; }
    public Dish? Dish { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
