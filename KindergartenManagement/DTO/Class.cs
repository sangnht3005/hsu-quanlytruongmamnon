using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class Class
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public Guid GradeId { get; set; }
    public Grade? Grade { get; set; }

    public int Capacity { get; set; }

    // Monthly tuition fee for this class
    public decimal TuitionFee { get; set; } = 0;

    // Monthly meal fee for this class
    public decimal MealFee { get; set; } = 0;

    public Guid? TeacherId { get; set; }
    public User? Teacher { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
