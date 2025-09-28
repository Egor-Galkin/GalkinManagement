using ManagementWpfApp.Helpers;
using ManagementWpfApp.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ManagementWpfApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private NavigationService _navigationService;
        private string _pageTitle;
        private Visibility _backButtonVisibility = Visibility.Collapsed;
        private readonly IServiceProvider _serviceProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BackCommand { get; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            BackCommand = new RelayCommand(OnBack, CanGoBack);
        }

        public void SetNavigationService(NavigationService navigationService)
        {
            if (_navigationService != null)
                _navigationService.Navigated -= NavigationService_Navigated;

            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _navigationService.Navigated += NavigationService_Navigated;

            UpdateBackButtonVisibility();
            ((RelayCommand)BackCommand).RaiseCanExecuteChanged();
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
                PageTitle = page.Title ?? "";
            else
                PageTitle = "";

            UpdateBackButtonVisibility();
            ((RelayCommand)BackCommand).RaiseCanExecuteChanged();
        }

        private void UpdateBackButtonVisibility()
        {
            BackButtonVisibility = (_navigationService != null && _navigationService.CanGoBack) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnBack()
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        private bool CanGoBack()
        {
            return _navigationService != null && _navigationService.CanGoBack;
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility BackButtonVisibility
        {
            get => _backButtonVisibility;
            set
            {
                if (_backButtonVisibility != value)
                {
                    _backButtonVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public LoginPage ResolveLoginPage()
        {
            return _serviceProvider.GetService(typeof(LoginPage)) as LoginPage;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}