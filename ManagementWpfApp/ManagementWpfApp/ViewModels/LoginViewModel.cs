using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ManagementWpfApp.Helpers;
using ManagementWpfApp.Services;

namespace ManagementWpfApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin, OnLoginException);
        }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public TaskCompletionSource<bool> LoginTaskCompletionSource { get; private set; }

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        private async Task LoginAsync()
        {
            LoginTaskCompletionSource = new TaskCompletionSource<bool>();

            var user = await _userService.AuthenticateAsync(Login, Password);
            if (user != null)
            {
                App.CurrentUser = user;
                LoginTaskCompletionSource.SetResult(true);
                OnLoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Пользователь с такими данными не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoginTaskCompletionSource.SetResult(false);
            }
        }

        private void OnLoginException(Exception ex)
        {
            MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public event EventHandler OnLoginSucceeded;
    }
}