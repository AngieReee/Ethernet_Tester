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
        public RelayCommand NavigateCurrentNetwork { get; set; }

        public CurrentNetworkViewModel(INavigationService navigation)
        {
        }
    }
}
