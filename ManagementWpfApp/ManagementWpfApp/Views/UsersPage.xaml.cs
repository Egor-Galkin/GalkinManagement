using System.Windows.Controls;
using ManagementWpfApp.Services;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp.Views
{
    public partial class UsersPage : Page
    {
        private readonly UsersViewModel _viewModel;

        public UsersPage()
        {
            InitializeComponent();
            var userService = new UserService(App.DbContext);
            _viewModel = new UsersViewModel(userService)
            {
                CurrentUser = App.CurrentUser
            };
            DataContext = _viewModel;
            Loaded += UsersPage_Loaded;
        }

        private async void UsersPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadUsersAsync();
        }
    }
}