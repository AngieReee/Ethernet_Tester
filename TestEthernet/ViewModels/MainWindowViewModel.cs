using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using TestEthernet.Core;
using TestEthernet.Services;
using TestEthernet.Views;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        #region [Переменные и их свойства]        

        List<IPAddress> detectedAddresses;
        public List<IPAddress> DetectedAddresses {
            get => detectedAddresses;
            set
            {
                detectedAddresses = value;
                OnPropertyChanged();
            }
        }

        string outputDescription;
        public string OutputDescription
        {
            get => outputDescription;
            set
            {
                outputDescription = value;
                OnPropertyChanged();
            }
        }

        string ipListDescription;
        public string IpListDescription
        {
            get => ipListDescription;
            set
            {
                ipListDescription = value;
                OnPropertyChanged();
            }
        }

        string[] addressData = new string[2];
        public string[] AddressData
        {
            get => addressData;
            set
            {
                addressData = value;
                OnPropertyChanged();
            }
        }

        string host;
        public string Host
        {
            get => host;
            set
            {
                host = value;
                OnPropertyChanged();
            }
        }

        IPAddress[] address;
        public IPAddress[] Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;

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



        /// <summary>
        /// Метод, в котором считываются данные о текущем хосте
        /// </summary>
        public string GetNetworkData()
        {
            string ipV4 = "";
            Host = Dns.GetHostName();
            Address = Dns.GetHostAddresses(Host);

            for (int i = 0; i < Address.Length; i++)
            {
                if (Address[i].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    AddressData[i] = "IPv6 - ";
                }
                else if (Address[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    AddressData[i] = "IPv4 - ";
                    ipV4 = Address[i].ToString();
                }

                AddressData[i] = AddressData[i] + Address[i].ToString();
            }

            return ipV4;
        }


        public List<IPAddress> CheckCurrentNetwork(string ipV4)
        {
            string[] parts = ipV4.Split('.');
            ipV4 = parts[0] + "." + parts[1] + "." + parts[2] + ".";
            int startAddress = 1;
            int endAddress = 50;

            List<IPAddress> detectedAddresses = new List<IPAddress>();
            Ping ping = new Ping();

            for(int addressPart = startAddress; addressPart<=endAddress; addressPart++)
            {
                string currentAddressAsString = $"{ipV4}{addressPart}";
                IPAddress currentAddress = IPAddress.Parse(currentAddressAsString);
                var pingResult = ping.Send(currentAddress, 700);
                Debug.WriteLine($"[{DateTime.Now}] {currentAddress}: {pingResult.Status}");

                if (pingResult.Status == IPStatus.Success)
                {
                    detectedAddresses.Add(currentAddress);
                }
            }

            return detectedAddresses;
        }


        public MainWindowViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateCurrentNetwork = new RelayCommand(execute: (object o) => { Navigation.NavigateTo<CurrentNetworkViewModel>(); },
            canExecute: (object o) => true);

            try
            {
                string ipV4 = GetNetworkData();
                OutputDescription = "Адреса хоста: ";
                DetectedAddresses = CheckCurrentNetwork(ipV4);
                if (detectedAddresses.Count != 0)
                {
                    IpListDescription = "Доступные IP адреса";
                }
                else
                {
                    IpListDescription = "Нет доступных IP адресов";
                }
            }
            catch
            {
                OutputDescription = "Ошибка подключения сети";
            }
            
        }

        public RelayCommand NavigateCurrentNetwork { get; set; }
    }
}
