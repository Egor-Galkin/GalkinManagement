using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.Helpers;
using ManagementWpfApp.Views;

namespace ManagementWpfApp.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        private User _currentUser;
        private ObservableCollection<User> _allUsers;
        private string _searchText;
        private ICollectionView _usersView;

        public UsersViewModel(UserService userService)
        {
            _userService = userService;

            LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
            EditCommand = new AsyncRelayCommand<User>(EditUserAsync);
            DeleteCommand = new AsyncRelayCommand<User>(DeleteUserAsync);
            AddCommand = new AsyncRelayCommand(AddUserAsync);

            SearchCommand = new AsyncRelayCommand<string>(OnSearchAsync);

            _allUsers = new ObservableCollection<User>();
            _usersView = CollectionViewSource.GetDefaultView(_allUsers);
            _usersView.Filter = UserFilter;
            Users = _usersView;
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICollectionView Users { get; }

        public ICommand LoadUsersCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SearchCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    _usersView.Refresh();
                }
            }
        }

        private bool UserFilter(object obj)
        {
            if (obj is User user)
            {
                if (string.IsNullOrEmpty(SearchText))
                    return true;

                var lowerSearch = SearchText.ToLower();
                return (user.Nickname != null && user.Nickname.ToLower().Contains(lowerSearch)) ||
                       (user.Login != null && user.Login.ToLower().Contains(lowerSearch));
            }
            return false;
        }

        private async Task OnSearchAsync(string searchText)
        {
            SearchText = searchText;
            await Task.CompletedTask;
        }

        public async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            _allUsers.Clear();
            foreach (var user in users)
            {
                _allUsers.Add(user);
            }
        }

        private async Task EditUserAsync(User user)
        {
            if (user == null) return;
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.Navigate(new UserEditPage(user));
            await Task.CompletedTask;
        }

        private async Task DeleteUserAsync(User user)
        {
            if (user == null) return;
            var result = MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _userService.DeleteUserAsync(user);
                _allUsers.Remove(user);
            }
        }

        private async Task AddUserAsync()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.Navigate(new UserEditPage(null));
            await Task.CompletedTask;
        }
    }
}