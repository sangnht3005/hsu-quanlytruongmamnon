using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
