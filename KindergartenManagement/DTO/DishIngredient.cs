using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class DishIngredient
{
    public Guid DishId { get; set; }
    public Dish? Dish { get; set; }

    public Guid IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    [Required]
    public decimal Quantity { get; set; } // in grams or ml

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
