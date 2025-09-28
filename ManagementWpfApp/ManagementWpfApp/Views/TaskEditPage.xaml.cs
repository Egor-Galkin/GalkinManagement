using System.Windows.Controls;
using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp.Views
{
    public partial class TaskEditPage : Page
    {
        public TaskEditPage(Mission mission, User currentUser)
        {
            InitializeComponent();
            var missionService = new MissionService(App.DbContext);
            DataContext = new TaskEditViewModel(missionService, currentUser, mission);
        }
    }
}