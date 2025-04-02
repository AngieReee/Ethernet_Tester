using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TestEthernet.Core;
using TestEthernet.Services;
using TestEthernet.ViewModels;
using TestEthernet.Views;

namespace TestEthernet
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType
            =>  (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var curWindow = _serviceProvider.GetRequiredService<MainWindow>();
            curWindow.Show();
            base.OnStartup(e);
        }
    }
}
