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

        string startAddress;
        public string StartAddress
        {
            get => startAddress;
            set
            {
                startAddress = value;
                OnPropertyChanged();
            }
        }

        string endAddress;
        public string EndAddress
        {
            get => endAddress;
            set
            {
                endAddress = value;
                OnPropertyChanged();
            }
        }

        string ip;
        public string Ip { get => ip;
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }

        string[] detectedMacs;
        public string[] DetectedMacs
        {
            get => detectedMacs;
            set
            {
                detectedMacs = value;
                OnPropertyChanged();
            }
        }

        string[] detectedHosts;
        public string[] DetectedHosts
        {
            get => detectedHosts;
            set
            {
                detectedHosts = value;
                OnPropertyChanged();
            }
        }

        List<IPAddress> detectedAddresses;
        public List<IPAddress> DetectedAddresses
        {
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

        string[] addressData;
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
        /// Метод, вычисляющий MAC адрес по IP
        /// </summary>
        /// <param name="ipAddress">Текущий IP адрес для вычисления</param>
        /// <returns>В зависимости от результатов вычисления метод возвращает либо MAC адрес, либо "Неопределено"</returns>
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a " + ipAddress;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                         + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                         + "-" + substrings[7] + "-"
                         + substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "Неопределено";
            }
        }

        /// <summary>
        /// Метод, возвращающий строковый массив с MAC адресами хостов
        /// </summary>
        /// <param name="detectedAddresses">Лист с IP адресами доступных хостов</param>
        /// <returns>В результате вычислений метод возвращает строковый массив с MAC адресами хостов</returns>
        public string[] GetMacsArray(List<IPAddress> detectedAddresses)
        {
            string[] s = new string[detectedAddresses.Count];

            for (int i = 0; i < detectedAddresses.Count; i++)
            {
                s[i] = GetMacAddress(detectedAddresses[i].ToString());
            }

            return s;
        }

        /// <summary>
        /// Метод, вычисляющий имя хоста по IP адресу
        /// </summary>
        /// <param name="ipAddress">Текущий IP адрес для вычисления</param>
        /// <returns>В зависимости от результатов вычисления метод возвращает либо имя хоста, либо IP адрес</returns>
        public string GetHostNameByIp(string ipAddress)
        {
            try
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry(ipAddress);
                if (iPHostEntry != null)
                {
                    return iPHostEntry.HostName;
                }
                else
                {
                    return ipAddress;
                }
            }
            catch(SocketException ex) { }

            return ipAddress;
        }

        /// <summary>
        /// Метод, возвращающий строковый массив с именами хостов
        /// </summary>
        /// <param name="detectedAddresses">Лист с IP адресами доступных хостов</param>
        /// <returns>В результате вычислений метод возвращает строковый массив с именами хостов</returns>
        public string[] GetNamesArray(List<IPAddress> detectedAddresses)
        {
            string[] s = new string[detectedAddresses.Count];

            for (int i = 0; i < detectedAddresses.Count; i++)
            {
                s[i] = GetHostNameByIp(detectedAddresses[i].ToString());
            }

            return s;
        }



        /// <summary>
        /// Метод, в котором считываются данные о текущем хосте
        /// </summary>
        public string GetNetworkData()
        {
            string ipV4 = "";
            Host = Dns.GetHostName();
            Address = Dns.GetHostAddresses(Host);
            Array.Resize<string>(ref addressData, address.Length);

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



        public void GetAllData()
        {
            DetectedAddresses = CheckCurrentNetwork(Ip);
            if (detectedAddresses.Count != 0)
            {
                IpListDescription = "";
                DetectedHosts = GetNamesArray(detectedAddresses);
                DetectedMacs = GetMacsArray(detectedAddresses);
            }
            else
            {
                IpListDescription = "Нет доступных IP адресов";
            }
        }

        /// <summary>
        /// Метод, который выводит доступные IP адреса
        /// </summary>
        /// <param name="ipV4">IP адрес формата v4 текущего компьютера</param>
        /// <returns></returns>
        public List<IPAddress> CheckCurrentNetwork(string ipV4)
        {
            /*string[] parts = ipV4.Split('.');
            ipV4 = parts[0] + "." + parts[1] + "." + parts[2] + ".";

            StartAddress = ipV4;
            EndAddress = ipV4;*/

            int startAddress = 1;
            int endAddress = 10;

            List<IPAddress> detectedAddresses = new List<IPAddress>();
            Ping ping = new Ping();

            for (int addressPart = startAddress; addressPart <= endAddress; addressPart++)
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
                OutputDescription = "Адреса хоста:";
                string[] parts = ipV4.Split('.');
                Ip = parts[0] + "." + parts[1] + "." + parts[2] + ".";

                StartAddress = Ip;
                EndAddress = Ip;
            }
            catch
            {
                OutputDescription = "Ошибка подключения сети";
            }
        }

        public RelayCommand NavigateCurrentNetwork { get; set; }
    }
}
