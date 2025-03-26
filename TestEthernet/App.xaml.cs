using Egor92.MvvmNavigation;
using System.Windows;
using TestEthernet.Views;
using TestEthernet.ViewModels;

namespace TestEthernet
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        { 
            var window = new MainWindow();
            var navigationManager = new NavigationManager(window);
            var viewModel = new MainWindow();
            window.DataContext = viewModel;

            navigationManager.Register<CurrentNetworkView>("CurrentNetworkKey", () => new CurrentNetworkViewModel(navigationManager));


            window.Show();

        }
    }
}
