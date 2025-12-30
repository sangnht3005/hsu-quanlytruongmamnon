using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

/// <summary>
/// Business Logic Object for Reporting module
/// Provides high-level operations for generating and retrieving reports
/// </summary>
public interface IReportingBlo
{
    // Student Reports
    Task<StudentReportDto?> GetStudentReportAsync(Guid studentId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<StudentReportDto>> GetStudentReportsByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null);
    
    // Class Reports
    Task<ClassReportDto?> GetClassReportAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<ClassReportDto>> GetAllClassReportsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    
    // Health & Vaccination Reports
    Task<HealthStatisticsDto> GetHealthStatisticsAsync(Guid? classId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<VaccinationStatisticsDto> GetVaccinationStatisticsAsync(Guid? classId = null);
    
    // Financial Reports
    Task<FinancialReportDto> GetFinancialReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<FinancialReportByClassDto> GetFinancialReportByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null);
}

public class ReportingBlo : IReportingBlo
{
    private readonly IReportingDao _reportingDao;

    public ReportingBlo(IReportingDao reportingDao)
    {
        _reportingDao = reportingDao;
    }

    public async Task<StudentReportDto?> GetStudentReportAsync(Guid studentId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetStudentReportAsync(studentId, fromDate, toDate);
    }

    public async Task<IEnumerable<StudentReportDto>> GetStudentReportsByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetStudentReportsByClassAsync(classId, fromDate, toDate);
    }

    public async Task<ClassReportDto?> GetClassReportAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetClassReportAsync(classId, fromDate, toDate);
    }

    public async Task<IEnumerable<ClassReportDto>> GetAllClassReportsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetAllClassReportsAsync(fromDate, toDate);
    }

    public async Task<HealthStatisticsDto> GetHealthStatisticsAsync(Guid? classId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetHealthStatisticsAsync(classId, fromDate, toDate);
    }

    public async Task<VaccinationStatisticsDto> GetVaccinationStatisticsAsync(Guid? classId = null)
    {
        return await _reportingDao.GetVaccinationStatisticsAsync(classId);
    }

    public async Task<FinancialReportDto> GetFinancialReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetFinancialReportAsync(fromDate, toDate);
    }

    public async Task<FinancialReportByClassDto> GetFinancialReportByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await _reportingDao.GetFinancialReportByClassAsync(classId, fromDate, toDate);
    }
}
