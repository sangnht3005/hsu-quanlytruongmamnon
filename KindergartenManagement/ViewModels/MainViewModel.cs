using System.Windows.Input;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;
    private Account? _currentAccount;

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        StudentManagementViewModel studentManagementViewModel,
        ClassManagementViewModel classManagementViewModel,
        AttendanceViewModel attendanceViewModel,
        HealthRecordViewModel healthRecordViewModel,
        MenuManagementViewModel menuManagementViewModel,
        InvoiceManagementViewModel invoiceManagementViewModel,
        AccountManagementViewModel accountManagementViewModel,
        StaffManagementViewModel staffManagementViewModel,
        ParentManagementViewModel parentManagementViewModel,
        StaffLeaveManagementViewModel staffLeaveManagementViewModel)
    {
        DashboardViewModel = dashboardViewModel;
        StudentManagementViewModel = studentManagementViewModel;
        ClassManagementViewModel = classManagementViewModel;
        AttendanceViewModel = attendanceViewModel;
        HealthRecordViewModel = healthRecordViewModel;
        MenuManagementViewModel = menuManagementViewModel;
        InvoiceManagementViewModel = invoiceManagementViewModel;
        AccountManagementViewModel = accountManagementViewModel;
        StaffManagementViewModel = staffManagementViewModel;
        ParentManagementViewModel = parentManagementViewModel;
        StaffLeaveManagementViewModel = staffLeaveManagementViewModel;

        NavigateCommand = new RelayCommand(Navigate);
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = DashboardViewModel;
    }

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set => SetProperty(ref _currentAccount, value);
    }

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public DashboardViewModel DashboardViewModel { get; }
    public StudentManagementViewModel StudentManagementViewModel { get; }
    public ClassManagementViewModel ClassManagementViewModel { get; }
    public AttendanceViewModel AttendanceViewModel { get; }
    public HealthRecordViewModel HealthRecordViewModel { get; }
    public MenuManagementViewModel MenuManagementViewModel { get; }
    public InvoiceManagementViewModel InvoiceManagementViewModel { get; }
    public AccountManagementViewModel AccountManagementViewModel { get; }
    public StaffManagementViewModel StaffManagementViewModel { get; }
    public ParentManagementViewModel ParentManagementViewModel { get; }
    public StaffLeaveManagementViewModel StaffLeaveManagementViewModel { get; }

    public ICommand NavigateCommand { get; }
    public ICommand LogoutCommand { get; }

    private void Navigate(object? parameter)
    {
        if (parameter is string viewName)
        {
            CurrentViewModel = viewName switch
            {
                "Dashboard" => DashboardViewModel,
                "Students" => StudentManagementViewModel,
                "Classes" => ClassManagementViewModel,
                "Attendance" => AttendanceViewModel,
                "Health" => HealthRecordViewModel,
                "Menu" => MenuManagementViewModel,
                "Invoice" => InvoiceManagementViewModel,
                "Accounts" => AccountManagementViewModel,
                "Staff" => StaffManagementViewModel,
                "Parents" => ParentManagementViewModel,
                "StaffLeave" => StaffLeaveManagementViewModel,
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
