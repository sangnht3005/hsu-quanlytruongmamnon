using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KindergartenManagement.Views;
using KindergartenManagement.ViewModels;
using KindergartenManagement.BLO;
using KindergartenManagement.DAO;

namespace KindergartenManagement;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register DbContext
                services.AddDbContext<KindergartenDbContext>();

                // Register DAOs
                services.AddScoped<IAccountDao, AccountDao>();
                services.AddScoped<IRoleDao, RoleDao>();
                services.AddScoped<IPermissionDao, PermissionDao>();
                services.AddScoped<IUserDao, UserDao>();
                services.AddScoped<IParentDao, ParentDao>();
                services.AddScoped<IStudentDao, StudentDao>();
                services.AddScoped<IClassDao, ClassDao>();
                services.AddScoped<IGradeDao, GradeDao>();
                services.AddScoped<IAttendanceDao, AttendanceDao>();
                services.AddScoped<IHealthRecordDao, HealthRecordDao>();
                services.AddScoped<IFoodDao, FoodDao>();
                services.AddScoped<IMenuDao, MenuDao>();
                services.AddScoped<IMealTicketDao, MealTicketDao>();
                services.AddScoped<IInvoiceDao, InvoiceDao>();
                services.AddScoped<IStaffLeaveDao, StaffLeaveDao>();
                services.AddScoped<IVaccineDao, VaccineDao>();
                services.AddScoped<IVaccinationRecordDao, VaccinationRecordDao>();
                services.AddScoped<IFoodManagementDao, FoodManagementDao>();
                services.AddScoped<IReportingDao, ReportingDao>();

                // Register Tuition Fee DAOs
                services.AddScoped<ITuitionFeeDao, TuitionFeeDao>();

                // Register BLOs
                services.AddScoped<IAccountBlo, AccountBlo>();
                services.AddScoped<IRoleBlo, RoleBlo>();
                services.AddScoped<IPermissionBlo, PermissionBlo>();
                services.AddScoped<IUserBlo, UserBlo>();
                services.AddScoped<IParentBlo, ParentBlo>();
                services.AddScoped<IStudentBlo, StudentBlo>();
                services.AddScoped<IClassBlo, ClassBlo>();
                services.AddScoped<IGradeBlo, GradeBlo>();
                services.AddScoped<IAttendanceBlo, AttendanceBlo>();
                services.AddScoped<IHealthRecordBlo, HealthRecordBlo>();
                services.AddScoped<IFoodBlo, FoodBlo>();
                services.AddScoped<IMenuBlo, MenuBlo>();
                services.AddScoped<IMealTicketBlo, MealTicketBlo>();
                services.AddScoped<IInvoiceBlo, InvoiceBlo>();
                services.AddScoped<IStaffLeaveBlo, StaffLeaveBlo>();
                services.AddScoped<IVaccineBlo, VaccineBlo>();
                services.AddScoped<IVaccinationRecordBlo, VaccinationRecordBlo>();
                services.AddScoped<IFoodManagementBlo, FoodManagementBlo>();
                services.AddScoped<IReportingBlo, ReportingBlo>();

                // Register Tuition Fee BLOs
                services.AddScoped<ITuitionFeeBlo, TuitionFeeBlo>();

                // Register ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<StudentManagementViewModel>();
                services.AddTransient<GradeManagementViewModel>();
                services.AddTransient<ClassManagementViewModel>();
                services.AddTransient<AttendanceManagementViewModel>();
                services.AddTransient<HealthRecordViewModel>();
                services.AddTransient<MenuManagementViewModel>();
                services.AddTransient<InvoiceManagementViewModel>();
                services.AddTransient<AccountManagementViewModel>();
                services.AddTransient<RolePermissionManagementViewModel>();
                services.AddTransient<StaffManagementViewModel>();
                services.AddTransient<ParentManagementViewModel>();
                services.AddTransient<StaffLeaveManagementViewModel>();
                services.AddTransient<VaccineManagementViewModel>();
                services.AddTransient<VaccinationRecordManagementViewModel>();
                services.AddTransient<HealthRecordManagementViewModel>();
                services.AddTransient<SupplierManagementViewModel>();
                services.AddTransient<IngredientManagementViewModel>();
                services.AddTransient<DishManagementViewModel>();
                services.AddTransient<DailyMenuManagementViewModel>();
                services.AddTransient<MealTicketManagementViewModel>();
                services.AddTransient<ReportingViewModel>();
                services.AddTransient<TuitionFeeManagementViewModel>();

                // Register Views
                services.AddTransient<MainWindow>();
                services.AddTransient<LoginWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Add global exception handlers
        AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
        {
            System.Diagnostics.Debug.WriteLine($"AppDomain UnhandledException: {ex.ExceptionObject}");
            MessageBox.Show($"Lỗi không mong muốn:\n\n{ex.ExceptionObject}", 
                "Lỗi ứng dụng", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        Application.Current.DispatcherUnhandledException += (s, ex) =>
        {
            System.Diagnostics.Debug.WriteLine($"DispatcherUnhandledException: {ex.Exception}");
            MessageBox.Show($"Lỗi không mong muốn:\n\n{ex.Exception.Message}", 
                "Lỗi ứng dụng", MessageBoxButton.OK, MessageBoxImage.Error);
            ex.Handled = true;
        };

        try
        {
            await _host.StartAsync();

            // Initialize database
            var dbContext = _host.Services.GetRequiredService<KindergartenDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            // Show login window
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnStartup Exception: {ex}");
            MessageBox.Show($"Lỗi khởi động ứng dụng:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                "Lỗi ứng dụng", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }

    public static T GetService<T>() where T : class
    {
        return ((App)Current)._host.Services.GetRequiredService<T>();
    }
}
