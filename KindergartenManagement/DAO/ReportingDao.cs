using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

/// <summary>
/// Data Access Object for Reporting module
/// Provides optimized queries for generating various reports
/// </summary>
public interface IReportingDao
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

public class ReportingDao : IReportingDao
{
    private readonly KindergartenDbContext _context;

    public ReportingDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<StudentReportDto?> GetStudentReportAsync(Guid studentId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var student = await _context.Students
            .Include(s => s.Class)
            .Include(s => s.Parent)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return null;

        var attendances = _context.Attendances
            .Where(a => a.StudentId == studentId);

        if (fromDate.HasValue)
            attendances = attendances.Where(a => a.Date >= fromDate.Value);
        if (toDate.HasValue)
            attendances = attendances.Where(a => a.Date <= toDate.Value);

        var attendanceCount = await attendances.CountAsync();
        var presentCount = await attendances.Where(a => a.Status == "Present").CountAsync();
        var absentCount = await attendances.Where(a => a.Status == "Absent").CountAsync();
        var lateCount = await attendances.Where(a => a.Status == "Late").CountAsync();

        var healthRecords = _context.HealthRecords
            .Where(h => h.StudentId == studentId);

        if (fromDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt <= toDate.Value);

        return new StudentReportDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            ClassName = student.Class?.Name,
            ParentName = student.Parent?.FullName,
            TotalAttendances = attendanceCount,
            PresentDays = presentCount,
            AbsentDays = absentCount,
            LateDays = lateCount,
            AttendancePercentage = attendanceCount > 0 ? Math.Round((double)presentCount / attendanceCount * 100, 2) : 0,
            HealthCheckCount = await healthRecords.CountAsync(),
            LastHealthCheckDate = await healthRecords.OrderByDescending(h => h.CreatedAt).Select(h => h.CreatedAt).FirstOrDefaultAsync(),
            GeneratedDate = DateTime.Now
        };
    }

    public async Task<IEnumerable<StudentReportDto>> GetStudentReportsByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var students = await _context.Students
            .Where(s => s.ClassId == classId)
            .Include(s => s.Class)
            .Include(s => s.Parent)
            .ToListAsync();

        var reports = new List<StudentReportDto>();

        foreach (var student in students)
        {
            var report = await GetStudentReportAsync(student.Id, fromDate, toDate);
            if (report != null)
                reports.Add(report);
        }

        return reports;
    }

    public async Task<ClassReportDto?> GetClassReportAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var classObj = await _context.Classes
            .Include(c => c.Grade)
            .FirstOrDefaultAsync(c => c.Id == classId);

        if (classObj == null)
            return null;

        var students = await _context.Students
            .Where(s => s.ClassId == classId)
            .CountAsync();

        var attendances = _context.Attendances
            .Where(a => a.ClassId == classId);

        if (fromDate.HasValue)
            attendances = attendances.Where(a => a.Date >= fromDate.Value);
        if (toDate.HasValue)
            attendances = attendances.Where(a => a.Date <= toDate.Value);

        var totalAttendances = await attendances.CountAsync();
        var presentCount = await attendances.Where(a => a.Status == "Present").CountAsync();

        var healthRecords = _context.HealthRecords
            .Where(h => h.Student!.ClassId == classId);

        if (fromDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt <= toDate.Value);

        return new ClassReportDto
        {
            ClassId = classObj.Id,
            ClassName = classObj.Name,
            GradeName = classObj.Grade?.Name,
            TotalStudents = students,
            AverageAttendancePercentage = totalAttendances > 0 ? Math.Round((double)presentCount / totalAttendances * 100, 2) : 0,
            TotalHealthChecks = await healthRecords.CountAsync(),
            ReportDate = DateTime.Now
        };
    }

    public async Task<IEnumerable<ClassReportDto>> GetAllClassReportsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var classes = await _context.Classes.ToListAsync();
        var reports = new List<ClassReportDto>();

        foreach (var classObj in classes)
        {
            var report = await GetClassReportAsync(classObj.Id, fromDate, toDate);
            if (report != null)
                reports.Add(report);
        }

        return reports;
    }

    public async Task<HealthStatisticsDto> GetHealthStatisticsAsync(Guid? classId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var healthRecords = _context.HealthRecords.AsQueryable();

        if (classId.HasValue)
            healthRecords = healthRecords.Where(h => h.Student!.ClassId == classId.Value);

        if (fromDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            healthRecords = healthRecords.Where(h => h.CreatedAt <= toDate.Value);

        var records = await healthRecords.ToListAsync();

        var avgHeight = 0.0;
        var avgWeight = 0.0;
        
        if (records.Any())
        {
            var heightValues = records.Where(r => r.Height.HasValue).Select(r => (double)r.Height.Value).ToList();
            var weightValues = records.Where(r => r.Weight.HasValue).Select(r => (double)r.Weight.Value).ToList();
            
            avgHeight = heightValues.Any() ? Math.Round(heightValues.Average(), 2) : 0;
            avgWeight = weightValues.Any() ? Math.Round(weightValues.Average(), 2) : 0;
        }

        return new HealthStatisticsDto
        {
            TotalHealthChecks = records.Count,
            AverageHeight = avgHeight,
            AverageWeight = avgWeight,
            HealthyCount = records.Count(r => r.GeneralHealth == "Good"),
            NeedAttentionCount = records.Count(r => r.GeneralHealth == "Fair" || r.GeneralHealth == "Poor"),
            ReportDate = DateTime.Now
        };
    }

    public async Task<VaccinationStatisticsDto> GetVaccinationStatisticsAsync(Guid? classId = null)
    {
        var students = classId.HasValue
            ? _context.Students.Where(s => s.ClassId == classId.Value)
            : _context.Students.AsQueryable();

        var totalStudents = await students.CountAsync();
        
        // Use health records as vaccination indicator (if a student has health records, consider them checked)
        var studentIds = await students.Select(s => s.Id).ToListAsync();
        var vaccinatedCount = await _context.HealthRecords
            .Where(h => studentIds.Contains(h.StudentId))
            .Select(h => h.StudentId)
            .Distinct()
            .CountAsync();

        return new VaccinationStatisticsDto
        {
            TotalStudents = totalStudents,
            VaccinatedCount = vaccinatedCount,
            NotVaccinatedCount = totalStudents - vaccinatedCount,
            VaccinationPercentage = totalStudents > 0 ? Math.Round((double)vaccinatedCount / totalStudents * 100, 2) : 0,
            ReportDate = DateTime.Now
        };
    }

    public async Task<FinancialReportDto> GetFinancialReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var invoices = _context.Invoices.AsQueryable();

        if (fromDate.HasValue)
            invoices = invoices.Where(i => i.IssueDate >= fromDate.Value);
        if (toDate.HasValue)
            invoices = invoices.Where(i => i.IssueDate <= toDate.Value);

        var invoiceList = await invoices.ToListAsync();

        var totalIncome = invoiceList.Sum(i => i.Amount);
        var paidIncome = invoiceList.Where(i => i.Status == "Đã thanh toán").Sum(i => i.Amount);
        var unpaidIncome = invoiceList.Where(i => i.Status == "Chưa thanh toán").Sum(i => i.Amount);

        return new FinancialReportDto
        {
            TotalInvoices = invoiceList.Count,
            TotalIncome = Math.Round((double)totalIncome, 2),
            PaidIncome = Math.Round((double)paidIncome, 2),
            UnpaidIncome = Math.Round((double)unpaidIncome, 2),
            AverageInvoiceAmount = invoiceList.Any() ? Math.Round((double)invoiceList.Average(i => i.Amount), 2) : 0,
            ReportDate = DateTime.Now
        };
    }

    public async Task<FinancialReportByClassDto> GetFinancialReportByClassAsync(Guid classId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var classObj = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
        if (classObj == null)
            throw new ArgumentException("Class not found");

        var invoices = _context.Invoices
            .Where(i => i.ClassId == classId);

        if (fromDate.HasValue)
            invoices = invoices.Where(i => i.IssueDate >= fromDate.Value);
        if (toDate.HasValue)
            invoices = invoices.Where(i => i.IssueDate <= toDate.Value);

        var invoiceList = await invoices.ToListAsync();

        return new FinancialReportByClassDto
        {
            ClassId = classId,
            ClassName = classObj.Name,
            TotalInvoices = invoiceList.Count,
            TotalIncome = Math.Round((double)invoiceList.Sum(i => i.Amount), 2),
            PaidIncome = Math.Round((double)invoiceList.Where(i => i.Status == "Đã thanh toán").Sum(i => i.Amount), 2),
            UnpaidIncome = Math.Round((double)invoiceList.Where(i => i.Status == "Chưa thanh toán").Sum(i => i.Amount), 2),
            ReportDate = DateTime.Now
        };
    }
}

// Report DTOs
public class StudentReportDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? ClassName { get; set; }
    public string? ParentName { get; set; }
    public int TotalAttendances { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public double AttendancePercentage { get; set; }
    public int HealthCheckCount { get; set; }
    public DateTime? LastHealthCheckDate { get; set; }
    public DateTime GeneratedDate { get; set; }
}

public class ClassReportDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? GradeName { get; set; }
    public int TotalStudents { get; set; }
    public double AverageAttendancePercentage { get; set; }
    public int TotalHealthChecks { get; set; }
    public DateTime ReportDate { get; set; }
}

public class HealthStatisticsDto
{
    public int TotalHealthChecks { get; set; }
    public double AverageHeight { get; set; }
    public double AverageWeight { get; set; }
    public int HealthyCount { get; set; }
    public int NeedAttentionCount { get; set; }
    public DateTime ReportDate { get; set; }
}

public class VaccinationStatisticsDto
{
    public int TotalStudents { get; set; }
    public int VaccinatedCount { get; set; }
    public int NotVaccinatedCount { get; set; }
    public double VaccinationPercentage { get; set; }
    public DateTime ReportDate { get; set; }
}

public class FinancialReportDto
{
    public int TotalInvoices { get; set; }
    public double TotalIncome { get; set; }
    public double PaidIncome { get; set; }
    public double UnpaidIncome { get; set; }
    public double AverageInvoiceAmount { get; set; }
    public DateTime ReportDate { get; set; }
}

public class FinancialReportByClassDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalInvoices { get; set; }
    public double TotalIncome { get; set; }
    public double PaidIncome { get; set; }
    public double UnpaidIncome { get; set; }
    public DateTime ReportDate { get; set; }
}
