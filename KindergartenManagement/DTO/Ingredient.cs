using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Ingredient
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; } // Protein, Vegetable, Fruit, Grain, Dairy, Spice

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty; // kg, g, liter, ml, pieces

    [Required]
    public decimal CaloriesPer100g { get; set; } // Calories per 100g/100ml

    [Required]
    public decimal UnitPrice { get; set; } // Price per unit

    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
}
