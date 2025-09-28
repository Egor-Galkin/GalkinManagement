using ManagementWpfApp.Models;
using ManagementWpfApp.Services;
using ManagementWpfApp.ViewModels;
using ManagementWpfApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ManagementWpfApp
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static ApplicationContext DbContext { get; } = new ApplicationContext();
        public static User CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // DbContext
            services.AddSingleton(DbContext);

            // Services
            services.AddTransient<UserService>();
            services.AddTransient<MissionService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<TasksViewModel>();
            services.AddTransient<TaskEditViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<UserEditViewModel>();
            services.AddTransient<MainWindowViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginPage>();
            services.AddTransient<TasksPage>();
            services.AddTransient<TaskEditPage>();
            services.AddTransient<UsersPage>();
            services.AddTransient<UserEditPage>();
        }
    }
}