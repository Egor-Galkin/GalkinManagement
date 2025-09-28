using System;
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
    public class TaskEditViewModel : BaseViewModel
    {
        private readonly MissionService _missionService;
        private Mission _currentMission;
        private readonly User _currentUser;

        public TaskEditViewModel(MissionService missionService, User currentUser, Mission mission)
        {
            _missionService = missionService;
            _currentUser = currentUser;
            _currentMission = mission ?? new Mission { CreationDate = DateTime.Now };

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            AllUsers = new ObservableCollection<User>();
            Employees = new ObservableCollection<User>();

            _ = LoadUsersAndSetAuthorAsync();
        }

        public Mission CurrentMission
        {
            get => _currentMission;
            set
            {
                _currentMission = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCompletedEnabled));
            }
        }

        public ObservableCollection<User> AllUsers { get; }

        public ObservableCollection<User> Employees { get; }

        public ICommand SaveCommand { get; }

        public bool IsCompletedEnabled => CurrentMission != null && CurrentMission.MissionId != 0;

        private async Task LoadUsersAndSetAuthorAsync()
        {
            var users = await _missionService.GetUsersAsync();

            AllUsers.Clear();
            Employees.Clear();

            foreach (var user in users)
            {
                AllUsers.Add(user);
                if (user.RoleId == 1)
                    Employees.Add(user);
            }

            OnPropertyChanged(nameof(AllUsers));
            OnPropertyChanged(nameof(Employees));

            if (CurrentMission.Author == null && _currentUser != null)
            {
                var author = AllUsers.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (author != null)
                    CurrentMission.Author = author;
            }

            OnPropertyChanged(nameof(CurrentMission));
        }

        private async Task SaveAsync()
        {
            if (CurrentMission == null)
                return;

            if (string.IsNullOrWhiteSpace(CurrentMission.Name))
            {
                MessageBox.Show("Название задачи обязательно для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentMission.Description))
            {
                MessageBox.Show("Описание задачи обязательно для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentMission.Performer == null)
            {
                MessageBox.Show("Исполнитель обязательно должен быть выбран", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentMission.Author = _currentUser;
            CurrentMission.AuthorId = _currentUser.UserId;

            await _missionService.SaveMissionAsync(CurrentMission);

            MessageBox.Show("Задача сохранена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.FrameMain.GoBack();
        }
    }
}