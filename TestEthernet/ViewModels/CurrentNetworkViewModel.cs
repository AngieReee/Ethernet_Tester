using Egor92.MvvmNavigation.Abstractions;
using Egor92.MvvmNavigation.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
