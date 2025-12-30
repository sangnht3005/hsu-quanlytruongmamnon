using System.Windows.Input;
using KindergartenManagement.DTO;
using KindergartenManagement.Services;
using System.Threading.Tasks;

namespace KindergartenManagement.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;
    private Account? _currentAccount;
    private string _currentPage = "Dashboard";
    private readonly IPermissionService _permissionService;

    // Permission properties
    private bool _canViewDashboard = true;
    private bool _canManageStudents = true;
    private bool _canManageClasses = true;
    private bool _canManageGrades = true;
    private bool _canManageStaff = true;
    private bool _canManageParents = true;
    private bool _canManageAccounts = true;
    private bool _canManageAttendance = true;
    private bool _canManageHealth = true;
    private bool _canManageFinance = true;
    private bool _canManageMenu = true;
    private bool _canManageLeave = true;
    private bool _canViewReports = true;

    public MainViewModel(
        IPermissionService permissionService,
        DashboardViewModel dashboardViewModel,
        StudentManagementViewModel studentManagementViewModel,
        GradeManagementViewModel gradeManagementViewModel,
        ClassManagementViewModel classManagementViewModel,
        AttendanceManagementViewModel attendanceManagementViewModel,
        HealthRecordViewModel healthRecordViewModel,
        MenuManagementViewModel menuManagementViewModel,
        InvoiceManagementViewModel invoiceManagementViewModel,
        AccountManagementViewModel accountManagementViewModel,
        RolePermissionManagementViewModel rolePermissionManagementViewModel,
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
        ReportingViewModel reportingViewModel,
        TuitionFeeManagementViewModel tuitionFeeManagementViewModel)
    {
        _permissionService = permissionService;
        
        DashboardViewModel = dashboardViewModel;
        StudentManagementViewModel = studentManagementViewModel;
        GradeManagementViewModel = gradeManagementViewModel;
        ClassManagementViewModel = classManagementViewModel;
        AttendanceManagementViewModel = attendanceManagementViewModel;
        HealthRecordViewModel = healthRecordViewModel;
        MenuManagementViewModel = menuManagementViewModel;
        InvoiceManagementViewModel = invoiceManagementViewModel;
        AccountManagementViewModel = accountManagementViewModel;
        RolePermissionManagementViewModel = rolePermissionManagementViewModel;
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
        TuitionFeeManagementViewModel = tuitionFeeManagementViewModel;

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
                
                // Load permissions
                _ = LoadPermissionsAsync();
            }
        }
    }

    // Permission properties for UI binding
    public bool CanViewDashboard
    {
        get => _canViewDashboard;
        set => SetProperty(ref _canViewDashboard, value);
    }

    public bool CanManageStudents
    {
        get => _canManageStudents;
        set => SetProperty(ref _canManageStudents, value);
    }

    public bool CanManageClasses
    {
        get => _canManageClasses;
        set => SetProperty(ref _canManageClasses, value);
    }

    public bool CanManageGrades
    {
        get => _canManageGrades;
        set => SetProperty(ref _canManageGrades, value);
    }

    public bool CanManageStaff
    {
        get => _canManageStaff;
        set => SetProperty(ref _canManageStaff, value);
    }

    public bool CanManageParents
    {
        get => _canManageParents;
        set => SetProperty(ref _canManageParents, value);
    }

    public bool CanManageAccounts
    {
        get => _canManageAccounts;
        set => SetProperty(ref _canManageAccounts, value);
    }

    public bool CanManageAttendance
    {
        get => _canManageAttendance;
        set => SetProperty(ref _canManageAttendance, value);
    }

    public bool CanManageHealth
    {
        get => _canManageHealth;
        set => SetProperty(ref _canManageHealth, value);
    }

    public bool CanManageFinance
    {
        get => _canManageFinance;
        set => SetProperty(ref _canManageFinance, value);
    }

    public bool CanManageMenu
    {
        get => _canManageMenu;
        set => SetProperty(ref _canManageMenu, value);
    }

    public bool CanManageLeave
    {
        get => _canManageLeave;
        set => SetProperty(ref _canManageLeave, value);
    }

    public bool CanViewReports
    {
        get => _canViewReports;
        set => SetProperty(ref _canViewReports, value);
    }

    private async Task LoadPermissionsAsync()
    {
        if (CurrentAccount == null) return;

        await _permissionService.LoadPermissionsAsync(CurrentAccount);

        // Update permission properties
        CanViewDashboard = _permissionService.HasPermission("ViewDashboard");
        CanManageStudents = _permissionService.HasPermission("ManageStudents");
        CanManageClasses = _permissionService.HasPermission("ManageClasses");
        CanManageGrades = _permissionService.HasPermission("ManageGrades");
        CanManageStaff = _permissionService.HasPermission("ManageStaff");
        CanManageParents = _permissionService.HasPermission("ManageParents");
        CanManageAccounts = _permissionService.HasPermission("ManageAccounts");
        CanManageAttendance = _permissionService.HasPermission("ManageAttendance");
        CanManageHealth = _permissionService.HasPermission("ManageHealth");
        CanManageFinance = _permissionService.HasPermission("ManageFinance");
        CanManageMenu = _permissionService.HasPermission("ManageMenu");
        CanManageLeave = _permissionService.HasPermission("ManageLeave");
        CanViewReports = _permissionService.HasPermission("ViewReports");
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
    public RolePermissionManagementViewModel RolePermissionManagementViewModel { get; }
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
    public TuitionFeeManagementViewModel TuitionFeeManagementViewModel { get; }

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
                "RolePermission" => RolePermissionManagementViewModel,
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
                "TuitionFeeManagement" => TuitionFeeManagementViewModel,
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
