using System.Windows;
using System.Windows.Input;
using KindergartenManagement.BLO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAccountBlo _accountBlo;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public LoginViewModel(IAccountBlo accountBlo)
    {
        _accountBlo = accountBlo;
        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoginCommand { get; }

    public Account? CurrentAccount { get; private set; }

    private bool CanLogin(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(Username) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsLoading;
    }

    private async Task LoginAsync(object? parameter)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var account = await _accountBlo.LoginAsync(Username, Password);

            if (account == null)
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return;
            }

            CurrentAccount = account;

            // Open main window
            var mainWindow = App.GetService<Views.MainWindow>();
            var mainViewModel = App.GetService<MainViewModel>();
            mainViewModel.CurrentAccount = account;
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();

            // Close login window
            Application.Current.Windows.OfType<Views.LoginWindow>().FirstOrDefault()?.Close();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
