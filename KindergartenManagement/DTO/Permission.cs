using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Permission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
