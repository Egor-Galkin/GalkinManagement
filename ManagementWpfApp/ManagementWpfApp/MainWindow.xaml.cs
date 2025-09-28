using System.Windows;
using ManagementWpfApp.ViewModels;

namespace ManagementWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            DataContext = mainWindowViewModel;

            mainWindowViewModel.SetNavigationService(this.FrameMain.NavigationService);

            this.FrameMain.Navigate(mainWindowViewModel.ResolveLoginPage());
        }
    }
}