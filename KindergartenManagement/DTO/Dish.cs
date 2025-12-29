using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Dish
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; } // Main Course, Soup, Salad, Dessert, Beverage

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Recipe { get; set; }

    public decimal TotalCalories { get; set; } // Calculated from ingredients

    public decimal TotalCost { get; set; } // Calculated from ingredients

    [Required]
    public decimal SellingPrice { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
    public ICollection<MenuDish> MenuDishes { get; set; } = new List<MenuDish>();
}
