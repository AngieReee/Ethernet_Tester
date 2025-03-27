using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using TestEthernet.Core;
using TestEthernet.Services;
using TestEthernet.Views;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        #region [Переменные и их свойства]

        string outputDescription;
        public string OutputDescription
        {
            get => outputDescription; set
            {
                outputDescription = value;
                OnPropertyChanged();
            }
        }

        string[] addressData = new string[2];
        public string[] AddressData
        {
            get => addressData; set
            {
                addressData = value;
                OnPropertyChanged();
            }
        }

        string host;
        public string Host
        {
            get => host; set
            {
                host = value;
                OnPropertyChanged();
            }
        }

        IPAddress[] address;
        public IPAddress[] Address
        {
            get => address; set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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

        #endregion



        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



        public void GetNetworkData()
        {
            Host = Dns.GetHostName();
            Address = Dns.GetHostAddresses(Host);

            for (int i = 0; i < Address.Length; i++)
            {
                if (Address[i].AddressFamily.ToString() == "InterNetworkV6")
                {
                    AddressData[i] = "IPv6 - ";
                }
                else if (Address[i].AddressFamily.ToString() == "InterNetwork")
                {
                    AddressData[i] = "IPv4 - ";
                }

                AddressData[i] = AddressData[i] + Address[i].ToString();
            }
        }

        public RelayCommand NavigateCurrentNetwork { get; set; }

        public MainWindowViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateCurrentNetwork = new RelayCommand(execute: (object o) => { Navigation.NavigateTo<CurrentNetworkViewModel>(); },
            canExecute: (object o) => true);

            try
            {
                GetNetworkData();
                OutputDescription = "Адреса хоста: ";
            }
            catch
            {
                OutputDescription = "Ошибка подключения сети";
            }


        }
    }
}
