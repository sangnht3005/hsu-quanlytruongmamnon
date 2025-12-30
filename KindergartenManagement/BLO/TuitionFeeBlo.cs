using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

public interface ITuitionFeeBlo
{
    Task<TuitionFeeDto?> GetByGradeIdAsync(Guid gradeId);
    Task<IEnumerable<TuitionFeeDto>> GetAllAsync();
    Task<TuitionFeeDto> CreateAsync(TuitionFeeDto tuitionFeeDto);
    Task<TuitionFeeDto> UpdateAsync(TuitionFeeDto tuitionFeeDto);
    Task DeleteAsync(Guid id);
}

public class TuitionFeeBlo : ITuitionFeeBlo
{
    private readonly ITuitionFeeDao _tuitionFeeDao;

    public TuitionFeeBlo(ITuitionFeeDao tuitionFeeDao)
    {
        _tuitionFeeDao = tuitionFeeDao;
    }

    public async Task<TuitionFeeDto?> GetByGradeIdAsync(Guid gradeId)
    {
        var tuitionFee = await _tuitionFeeDao.GetByGradeIdAsync(gradeId);
        return tuitionFee == null ? null : MapToDto(tuitionFee);
    }

    public async Task<IEnumerable<TuitionFeeDto>> GetAllAsync()
    {
        var tuitionFees = await _tuitionFeeDao.GetAllAsync();
        return tuitionFees.Select(MapToDto);
    }

    public async Task<TuitionFeeDto> CreateAsync(TuitionFeeDto tuitionFeeDto)
    {
        var entity = new KindergartenManagement.DTO.TuitionFee
        {
            Id = Guid.NewGuid(),
            GradeId = tuitionFeeDto.GradeId,
            MonthlyTuitionFee = tuitionFeeDto.MonthlyTuitionFee,
            DailyMealFee = tuitionFeeDto.DailyMealFee,
            SchoolDaysPerMonth = tuitionFeeDto.SchoolDaysPerMonth,
            EffectiveDate = tuitionFeeDto.EffectiveDate
        };

        var result = await _tuitionFeeDao.CreateAsync(entity);
        return MapToDto(result);
    }

    public async Task<TuitionFeeDto> UpdateAsync(TuitionFeeDto tuitionFeeDto)
    {
        var entity = new KindergartenManagement.DTO.TuitionFee
        {
            Id = tuitionFeeDto.Id,
            GradeId = tuitionFeeDto.GradeId,
            MonthlyTuitionFee = tuitionFeeDto.MonthlyTuitionFee,
            DailyMealFee = tuitionFeeDto.DailyMealFee,
            SchoolDaysPerMonth = tuitionFeeDto.SchoolDaysPerMonth,
            EffectiveDate = tuitionFeeDto.EffectiveDate
        };

        var result = await _tuitionFeeDao.UpdateAsync(entity);
        return MapToDto(result);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _tuitionFeeDao.DeleteAsync(id);
    }

    private static TuitionFeeDto MapToDto(KindergartenManagement.DTO.TuitionFee tuitionFee)
    {
        return new TuitionFeeDto
        {
            Id = tuitionFee.Id,
            GradeId = tuitionFee.GradeId,
            GradeName = tuitionFee.Grade?.Name ?? string.Empty,
            MonthlyTuitionFee = tuitionFee.MonthlyTuitionFee,
            DailyMealFee = tuitionFee.DailyMealFee,
            SchoolDaysPerMonth = tuitionFee.SchoolDaysPerMonth,
            EffectiveDate = tuitionFee.EffectiveDate
        };
    }
}

public class TuitionFeeDto
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public decimal MonthlyTuitionFee { get; set; }
    public decimal DailyMealFee { get; set; }
    public int SchoolDaysPerMonth { get; set; } = 20;
    public DateTime EffectiveDate { get; set; }
}
