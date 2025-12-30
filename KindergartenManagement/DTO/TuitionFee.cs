using System.ComponentModel.DataAnnotations;

namespace KindergartenManagement.DTO;

public class TuitionFee
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid GradeId { get; set; }
    public Grade? Grade { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlyTuitionFee { get; set; } // Học phí hàng tháng

    [Required]
    [Range(0, double.MaxValue)]
    public decimal DailyMealFee { get; set; } // Tiền ăn trung bình/ngày

    [Required]
    [Range(1, 31)]
    public int SchoolDaysPerMonth { get; set; } = 20; // Số ngày học/tháng (để tính tiền ăn)

    public DateTime EffectiveDate { get; set; } = DateTime.Now;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
