using Egor92.MvvmNavigation.Abstractions;

namespace TestEthernet.ViewModels
{
    public class CurrentNetworkViewModel
    {
        private readonly INavigationManager _navigationManager;

        public CurrentNetworkViewModel(INavigationManager navigation)
        {
            _navigationManager = navigation;
        }
    }
}
