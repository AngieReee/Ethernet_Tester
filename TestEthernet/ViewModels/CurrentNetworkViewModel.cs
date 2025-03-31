using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEthernet.Core;
using TestEthernet.Services;

namespace TestEthernet.ViewModels
{
    public class CurrentNetworkViewModel : ViewModel
    {

        /*private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropityChanged();
            }

        }*/

        public RelayCommand NavigateCurrentNetwork { get; set; }

        public CurrentNetworkViewModel(INavigationService navigation)
        {
            /*Navigation = navigation;
            NavigateCurrentNetwork = new RelayCommand(execute: (object o) => { Navigation.NavigateTo<CurrentNetworkViewModel>(); },
            canExecute: (object o) => true);*/
        }
    }
}
