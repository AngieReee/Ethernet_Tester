using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TestEthernet.ViewModels;

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
            services.AddSingleton<CurrentNetworkViewModel>();

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
