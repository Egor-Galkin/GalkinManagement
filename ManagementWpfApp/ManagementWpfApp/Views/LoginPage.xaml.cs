using System;
using System.Windows;
using System.Windows.Controls;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp.Views
{
    public partial class LoginPage : Page
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.OnLoginSucceeded += ViewModel_OnLoginSucceeded;
        }

        private void ViewModel_OnLoginSucceeded(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow is ManagementWpfApp.MainWindow mainWindow)
            {
                mainWindow.FrameMain.Navigate(new TasksPage());
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}