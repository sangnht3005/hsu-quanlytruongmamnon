using System.Threading.Tasks;
using System.Windows.Input;
using KindergartenManagement.DAO;
using Microsoft.EntityFrameworkCore;

namespace KindergartenManagement.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly KindergartenDbContext _context;

    private bool _isLoading;
    private int _studentsCount;
    private int _classesCount;
    private int _gradesCount;
    private int _teachersCount;
    private int _parentsCount;
    private int _attendanceTodayPresent;
    private int _attendanceTodayAbsent;
    private int _invoicesPending;
    private int _invoicesOverdue;
    private decimal _tuitionPaidThisMonth;

    public DashboardViewModel(KindergartenDbContext context)
    {
        _context = context;
        RefreshCommand = new AsyncRelayCommand(_ => LoadDashboardAsync());

        // Auto-load when constructed
        _ = LoadDashboardAsync();
    }

    public string WelcomeMessage => "Chào mừng đến với hệ thống quản lý mầm non";

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public int StudentsCount
    {
        get => _studentsCount;
        set => SetProperty(ref _studentsCount, value);
    }

    public int ClassesCount
    {
        get => _classesCount;
        set => SetProperty(ref _classesCount, value);
    }

    public int GradesCount
    {
        get => _gradesCount;
        set => SetProperty(ref _gradesCount, value);
    }

    public int TeachersCount
    {
        get => _teachersCount;
        set => SetProperty(ref _teachersCount, value);
    }

    public int ParentsCount
    {
        get => _parentsCount;
        set => SetProperty(ref _parentsCount, value);
    }

    public int AttendanceTodayPresent
    {
        get => _attendanceTodayPresent;
        set => SetProperty(ref _attendanceTodayPresent, value);
    }

    public int AttendanceTodayAbsent
    {
        get => _attendanceTodayAbsent;
        set => SetProperty(ref _attendanceTodayAbsent, value);
    }

    public int InvoicesPending
    {
        get => _invoicesPending;
        set => SetProperty(ref _invoicesPending, value);
    }

    public int InvoicesOverdue
    {
        get => _invoicesOverdue;
        set => SetProperty(ref _invoicesOverdue, value);
    }

    public decimal TuitionPaidThisMonth
    {
        get => _tuitionPaidThisMonth;
        set => SetProperty(ref _tuitionPaidThisMonth, value);
    }

    public ICommand RefreshCommand { get; }

    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;

            var today = DateTime.Today;
            var month = today.Month;
            var year = today.Year;

            StudentsCount = await _context.Students.CountAsync();
            ClassesCount = await _context.Classes.CountAsync();
            GradesCount = await _context.Grades.CountAsync();
            TeachersCount = await _context.Users.CountAsync(u => u.Position != null && u.Position.Contains("Giáo viên"));
            ParentsCount = await _context.Parents.CountAsync();

            AttendanceTodayPresent = await _context.Attendances
                .CountAsync(a => a.Date.Date == today && a.Status == "Present");

            AttendanceTodayAbsent = await _context.Attendances
                .CountAsync(a => a.Date.Date == today && a.Status == "Absent");

            InvoicesPending = await _context.Invoices
                .CountAsync(i => i.Status == "Chưa thanh toán");

            InvoicesOverdue = await _context.Invoices
                .CountAsync(i => i.Status == "Quá hạn" ||
                                 (i.Status == "Chưa thanh toán" && i.DueDate != null && i.DueDate < today));

            TuitionPaidThisMonth = await _context.Invoices
                .Where(i => i.PaidDate != null && i.PaidDate.Value.Month == month && i.PaidDate.Value.Year == year)
                .SumAsync(i => (decimal?)i.Amount) ?? 0m;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Dashboard] Load error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}

