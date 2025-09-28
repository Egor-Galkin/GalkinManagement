using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.Helpers;
using System.Windows;

namespace ManagementWpfApp.ViewModels
{
    public class UserEditViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly User _currentUser;
        private readonly User _editingUser;
        private bool _isRoleEditable;
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;

        public UserEditViewModel(UserService userService, User user = null)
        {
            _userService = userService;
            _currentUser = App.CurrentUser;
            _editingUser = user ?? new User();

            Roles = new ObservableCollection<Role>();

            LoadRolesCommand = new AsyncRelayCommand(LoadRolesAsync);
            SaveCommand = new AsyncRelayCommand(SaveAsync);

            _ = LoadRolesAsync();

            OnPropertyChanged(nameof(CurrentUser));
            UpdateRoleEditable();
        }

        public User CurrentUser
        {
            get => _editingUser;
            private set { }
        }

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set { _roles = value; OnPropertyChanged(); }
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole != value)
                {
                    _selectedRole = value;
                    OnPropertyChanged();
                    if (_editingUser != null)
                        _editingUser.Role = _selectedRole;
                }
            }
        }

        public bool IsRoleEditable
        {
            get => _isRoleEditable;
            private set { _isRoleEditable = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand LoadRolesCommand { get; }

        private async Task LoadRolesAsync()
        {
            var roles = await _userService.GetRolesAsync();
            Roles.Clear();
            foreach (var role in roles)
                Roles.Add(role);

            if (_editingUser != null && _editingUser.UserId != 0 && _editingUser.Role != null)
            {
                SelectedRole = Roles.FirstOrDefault(r => r.RoleId == _editingUser.Role.RoleId);
            }
            else
            {
                SelectedRole = null;
            }
        }

        private void UpdateRoleEditable()
        {
            if (_editingUser.Role != null && _editingUser.Role.Name == "Администратор")
            {
                IsRoleEditable = false;
                return;
            }

            if (_editingUser.UsingCount > 0)
            {
                IsRoleEditable = false;
                return;
            }

            IsRoleEditable = true;
        }

        private async Task SaveAsync()
        {
            if (_editingUser == null)
                return;

            if (string.IsNullOrWhiteSpace(_editingUser.Nickname) ||
                string.IsNullOrWhiteSpace(_editingUser.Login) ||
                string.IsNullOrWhiteSpace(_editingUser.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedRole == null)
            {
                MessageBox.Show("Пожалуйста, выберите роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingUser = (await _userService.GetUsersAsync())
                .FirstOrDefault(u => u.Login == _editingUser.Login && u.UserId != _editingUser.UserId);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsRoleEditable)
            {
                _editingUser.Role = SelectedRole;
            }

            await _userService.SaveUserAsync(_editingUser);

            MessageBox.Show("Пользователь успешно сохранён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.GoBack();
        }
    }
}