using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class MenuFood
{
    public Guid MenuId { get; set; }
    public Menu? Menu { get; set; }

    public Guid FoodId { get; set; }
    public Food? Food { get; set; }

    public decimal Quantity { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
