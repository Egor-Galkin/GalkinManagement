using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.Helpers;
using System.Windows;
using ManagementWpfApp.Views;

namespace ManagementWpfApp.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private readonly MissionService _missionService;

        private User _currentUser;

        public TasksViewModel(MissionService missionService)
        {
            _missionService = missionService;
            Missions = new ObservableCollection<Mission>();

            EditCommand = new AsyncRelayCommand<Mission>(EditMissionAsync, CanEditMission);
            DeleteCommand = new AsyncRelayCommand<Mission>(DeleteMissionAsync, CanDeleteMission);
            CompleteCommand = new AsyncRelayCommand<Mission>(CompleteMissionAsync, CanCompleteMission);
            AddCommand = new AsyncRelayCommand(AddMissionAsync, CanAddMission);
            NavigateUsersCommand = new AsyncRelayCommand(NavigateUsersAsync);
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

                    OnPropertyChanged(nameof(UsersButtonVisibility));
                    OnPropertyChanged(nameof(AddButtonVisibility));

                    UpdateAccessProperties();
                }
            }
        }

        public ObservableCollection<Mission> Missions { get; set; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CompleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand NavigateUsersCommand { get; }

        public Visibility UsersButtonVisibility =>
            CurrentUser?.Role?.Name == "Администратор" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AddButtonVisibility =>
            (CurrentUser?.Role?.Name == "Администратор" || CurrentUser?.Role?.Name == "Руководитель") ? Visibility.Visible : Visibility.Collapsed;

        private bool CanEditMission(Mission mission) => IsAdminOrManager;
        private bool CanDeleteMission(Mission mission) => IsAdminOrManager;
        private bool CanCompleteMission(Mission mission) => IsUser && mission != null && !mission.Completed;
        private bool CanAddMission() => IsAdminOrManager;

        private bool IsAdminOrManager => CurrentUser?.Role?.Name == "Администратор" || CurrentUser?.Role?.Name == "Руководитель";
        private bool IsUser => CurrentUser?.Role?.Name == "Сотрудник";

        private void UpdateAccessProperties()
        {
            OnPropertyChanged(nameof(IsAdminOrManager));
            OnPropertyChanged(nameof(IsUser));
        }

        public async Task LoadMissionsAsync()
        {
            if (CurrentUser == null)
                return;

            var missions = await _missionService.GetMissionsForUserAsync(CurrentUser);
            Missions.Clear();

            foreach (var mission in missions)
                Missions.Add(mission);
        }

        private Task EditMissionAsync(Mission mission)
        {
            if (mission == null) return Task.CompletedTask;

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.Navigate(new TaskEditPage(mission, CurrentUser));

            return Task.CompletedTask;
        }

        private async Task DeleteMissionAsync(Mission mission)
        {
            if (mission == null)
                return;

            var result = MessageBox.Show("Удалить задачу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _missionService.DeleteMissionAsync(mission);
                Missions.Remove(mission);
            }
        }

        private async Task CompleteMissionAsync(Mission mission)
        {
            if (mission == null) return;
            mission.Completed = true;
            await _missionService.SaveMissionAsync(mission);
            await LoadMissionsAsync();
        }

        private async Task AddMissionAsync()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.Navigate(new TaskEditPage(null, CurrentUser));
            await Task.CompletedTask;
        }

        private async Task NavigateUsersAsync()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.Navigate(new UsersPage());
            await Task.CompletedTask;
        }
    }
}