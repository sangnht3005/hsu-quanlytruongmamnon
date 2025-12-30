using System.Windows.Input;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;
    private Account? _currentAccount;
    private string _currentPage = "Dashboard";

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        StudentManagementViewModel studentManagementViewModel,
        GradeManagementViewModel gradeManagementViewModel,
        ClassManagementViewModel classManagementViewModel,
        AttendanceManagementViewModel attendanceManagementViewModel,
        HealthRecordViewModel healthRecordViewModel,
        MenuManagementViewModel menuManagementViewModel,
        InvoiceManagementViewModel invoiceManagementViewModel,
        AccountManagementViewModel accountManagementViewModel,
        StaffManagementViewModel staffManagementViewModel,
        ParentManagementViewModel parentManagementViewModel,
        StaffLeaveManagementViewModel staffLeaveManagementViewModel,
        VaccineManagementViewModel vaccineManagementViewModel,
        VaccinationRecordManagementViewModel vaccinationRecordManagementViewModel,
        HealthRecordManagementViewModel healthRecordManagementViewModel,
        SupplierManagementViewModel supplierManagementViewModel,
        IngredientManagementViewModel ingredientManagementViewModel,
        DishManagementViewModel dishManagementViewModel,
        DailyMenuManagementViewModel dailyMenuManagementViewModel,
        MealTicketManagementViewModel mealTicketManagementViewModel,
        ReportingViewModel reportingViewModel)
    {
        DashboardViewModel = dashboardViewModel;
        StudentManagementViewModel = studentManagementViewModel;
        GradeManagementViewModel = gradeManagementViewModel;
        ClassManagementViewModel = classManagementViewModel;
        AttendanceManagementViewModel = attendanceManagementViewModel;
        HealthRecordViewModel = healthRecordViewModel;
        MenuManagementViewModel = menuManagementViewModel;
        InvoiceManagementViewModel = invoiceManagementViewModel;
        AccountManagementViewModel = accountManagementViewModel;
        StaffManagementViewModel = staffManagementViewModel;
        ParentManagementViewModel = parentManagementViewModel;
        StaffLeaveManagementViewModel = staffLeaveManagementViewModel;
        VaccineManagementViewModel = vaccineManagementViewModel;
        VaccinationRecordManagementViewModel = vaccinationRecordManagementViewModel;
        HealthRecordManagementViewModel = healthRecordManagementViewModel;
        SupplierManagementViewModel = supplierManagementViewModel;
        IngredientManagementViewModel = ingredientManagementViewModel;
        DishManagementViewModel = dishManagementViewModel;
        DailyMenuManagementViewModel = dailyMenuManagementViewModel;
        MealTicketManagementViewModel = mealTicketManagementViewModel;
        ReportingViewModel = reportingViewModel;

        NavigateCommand = new RelayCommand(Navigate);
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = DashboardViewModel;
    }

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            if (SetProperty(ref _currentAccount, value))
            {
                // Propagate current account to StaffLeaveManagementViewModel
                StaffLeaveManagementViewModel.CurrentAccount = value;
            }
        }
    }

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public DashboardViewModel DashboardViewModel { get; }
    public StudentManagementViewModel StudentManagementViewModel { get; }
    public GradeManagementViewModel GradeManagementViewModel { get; }
    public ClassManagementViewModel ClassManagementViewModel { get; }
    public AttendanceManagementViewModel AttendanceManagementViewModel { get; }
    public HealthRecordViewModel HealthRecordViewModel { get; }
    public MenuManagementViewModel MenuManagementViewModel { get; }
    public InvoiceManagementViewModel InvoiceManagementViewModel { get; }
    public AccountManagementViewModel AccountManagementViewModel { get; }
    public StaffManagementViewModel StaffManagementViewModel { get; }
    public ParentManagementViewModel ParentManagementViewModel { get; }
    public StaffLeaveManagementViewModel StaffLeaveManagementViewModel { get; }
    public VaccineManagementViewModel VaccineManagementViewModel { get; }
    public VaccinationRecordManagementViewModel VaccinationRecordManagementViewModel { get; }
    public HealthRecordManagementViewModel HealthRecordManagementViewModel { get; }
    public SupplierManagementViewModel SupplierManagementViewModel { get; }
    public IngredientManagementViewModel IngredientManagementViewModel { get; }
    public DishManagementViewModel DishManagementViewModel { get; }
    public DailyMenuManagementViewModel DailyMenuManagementViewModel { get; }
    public MealTicketManagementViewModel MealTicketManagementViewModel { get; }
    public ReportingViewModel ReportingViewModel { get; }

    public ICommand NavigateCommand { get; }
    public ICommand LogoutCommand { get; }

    private void Navigate(object? parameter)
    {
        if (parameter is string viewName)
        {
            CurrentPage = viewName;
            CurrentViewModel = viewName switch
            {
                "Dashboard" => DashboardViewModel,
                "Students" => StudentManagementViewModel,
                "Grades" => GradeManagementViewModel,
                "Classes" => ClassManagementViewModel,
                "Attendance" => AttendanceManagementViewModel,
                "Health" => HealthRecordViewModel,
                "Menu" => MenuManagementViewModel,
                "Invoice" => InvoiceManagementViewModel,
                "Accounts" => AccountManagementViewModel,
                "Vaccines" => VaccineManagementViewModel,
                "VaccinationRecords" => VaccinationRecordManagementViewModel,
                "HealthRecords" => HealthRecordManagementViewModel,
                "Staff" => StaffManagementViewModel,
                "Parents" => ParentManagementViewModel,
                "StaffLeave" => StaffLeaveManagementViewModel,
                "Suppliers" => SupplierManagementViewModel,
                "Ingredients" => IngredientManagementViewModel,
                "Dishes" => DishManagementViewModel,
                "DailyMenus" => DailyMenuManagementViewModel,
                "MealTickets" => MealTicketManagementViewModel,
                "Reporting" => ReportingViewModel,
                _ => DashboardViewModel
            };
        }
    }

    private void Logout(object? parameter)
    {
        CurrentAccount = null;
        
        var loginWindow = App.GetService<Views.LoginWindow>();
        loginWindow.Show();

        System.Windows.Application.Current.Windows
            .OfType<Views.MainWindow>()
            .FirstOrDefault()?.Close();
    }
}
