using System.Windows.Controls;
using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp.Views
{
    public partial class UserEditPage : Page
    {
        public UserEditPage(User user = null)
        {
            InitializeComponent();
            var userService = new UserService(App.DbContext);
            DataContext = new UserEditViewModel(userService, user);
        }
    }
}