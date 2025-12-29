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

                // Register Views
                services.AddTransient<MainWindow>();
                services.AddTransient<LoginWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        // Initialize database
        var dbContext = _host.Services.GetRequiredService<KindergartenDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Seed database with sample data
        var seeder = new Utilities.DatabaseSeeder(dbContext);
        await seeder.SeedAsync();

        // Show login window
        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
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
