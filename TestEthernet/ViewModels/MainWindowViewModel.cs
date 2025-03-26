using System.ComponentModel;
using TestEthernet.Core;
using TestEthernet.Services;
using TestEthernet.Views;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
                {
                    _navigation = value;
                    OnPropityChanged();
                }

        }

        public RelayCommand NavigateCurrentNetwork {  get; set; }


        public MainWindowViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateCurrentNetwork = new RelayCommand(execute: (object o) => { Navigation.NavigateTo<CurrentNetworkViewModel>(); },
            canExecute: (object o) => true);
            /*var networkData = NetworkInterface.GetAllNetworkInterfaces();*/
        }
    }
}
