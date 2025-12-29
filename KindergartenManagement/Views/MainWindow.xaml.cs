using System.Windows;
using KindergartenManagement.ViewModels;

namespace KindergartenManagement.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
