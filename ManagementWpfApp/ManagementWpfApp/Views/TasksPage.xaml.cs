using System.Windows.Controls;
using ManagementWpfApp.Services;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp.Views
{
    public partial class TasksPage : Page
    {
        private readonly TasksViewModel _viewModel;

        public TasksPage()
        {
            InitializeComponent();
            var missionService = new MissionService(App.DbContext);
            _viewModel = new TasksViewModel(missionService)
            {
                CurrentUser = App.CurrentUser
            };
            DataContext = _viewModel;
            Loaded += TasksPage_Loaded;
        }

        private async void TasksPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadMissionsAsync();
        }
    }
}