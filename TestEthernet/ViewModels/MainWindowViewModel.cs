using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TestEthernet.Core;
using TestEthernet.Models;
using TestEthernet.Services;
using TestEthernet.Views;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        #region [Переменные и их свойства]

        int startPort = IPEndPoint.MinPort;
        public int StartPort
        {
            get => startPort;
            set
            {
                startPort = value;
                OnPropertyChanged(nameof(StartPort));
            }
        }

        int endPort = 2000;
        public int EndPort
        {
            get => endPort;
            set
            {
                endPort = value;
                OnPropertyChanged(nameof(EndPort));
            }
        }

        string exceptionText;
        public string ExceptionText
        {
            get => exceptionText; set
            {
                exceptionText = value;
                OnPropertyChanged(nameof(ExceptionText));
            }
        }

        string isVisible = "Hidden";
        public string IsVisible
        {
            get => isVisible; set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        ICommand allOutput;
        public ICommand AllOutput
        {
            get
            {
                if (allOutput == null)
                {
                    allOutput = new RelayCommand(
                        x => this.GetAllData(),
                        x => true);
                }
                return allOutput;
            }
            set
            {
                allOutput = value;
                OnPropertyChanged(nameof(AllOutput));
            }
        }

        ICommand ports;
        public ICommand Ports
        {
            get
            {
                if (ports == null)
                {
                    ports = new RelayCommand(
                        x => this.GetPorts(CurIp),
                        x => true);
                }
                return ports;
            }
            set
            {
                ports = value;
                OnPropertyChanged(nameof(Ports));
            }
        }

        IPs curIp;
        public IPs CurIp
        {
            get => curIp;
            set
            {
                curIp = value;
                OnPropertyChanged(nameof(CurIp));
            }
        }

        string startAddress;
        public string StartAddress
        {
            get => startAddress;
            set
            {
                startAddress = value;
                OnPropertyChanged(nameof(StartAddress));
            }
        }

        string endAddress;
        public string EndAddress
        {
            get => endAddress;
            set
            {
                endAddress = value;
                OnPropertyChanged(nameof(EndAddress));
            }
        }

        string ip;
        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        ObservableCollection<string> detectedMacs;
        public ObservableCollection<string> DetectedMacs
        {
            get => detectedMacs;
            set
            {
                detectedMacs = value;
                OnPropertyChanged(nameof(DetectedMacs));
            }
        }

        ObservableCollection<string> detectedHosts;
        public ObservableCollection<string> DetectedHosts
        {
            get => detectedHosts;
            set
            {
                detectedHosts = value;
                OnPropertyChanged(nameof(DetectedHosts));
            }
        }

        ObservableCollection<IPs> detectedAddresses;
        public ObservableCollection<IPs> DetectedAddresses
        {
            get => detectedAddresses;
            set
            {
                detectedAddresses = value;
                OnPropertyChanged(nameof(DetectedAddresses));
            }
        }

        string outputDescription;
        public string OutputDescription
        {
            get => outputDescription;
            set
            {
                outputDescription = value;
                OnPropertyChanged(nameof(OutputDescription));
            }
        }

        string ipListDescription;
        public string IpListDescription
        {
            get => ipListDescription;
            set
            {
                ipListDescription = value;
                OnPropertyChanged(nameof(IpListDescription));
            }
        }

        string[] addressData;
        public string[] AddressData
        {
            get => addressData;
            set
            {
                addressData = value;
                OnPropertyChanged(nameof(AddressData));
            }
        }

        string host;
        public string Host
        {
            get => host;
            set
            {
                host = value;
                OnPropertyChanged(nameof(Host));
            }
        }

        IPAddress[] address;
        public IPAddress[] Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;

        #endregion

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
            catch (SocketException ex) { }

            return ipAddress;
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


        /// <summary>
        /// Метод, который выводит все данные о доступных интерфейсах сети
        /// </summary>
        public void GetAllData()
        {
            try
            {
                DetectedAddresses = CheckCurrentNetwork(Ip);
                if (DetectedAddresses.Count != 0)
                {
                    IpListDescription = "";
                    IsVisible = "Visible";
                    /*GetPorts();*/
                }
                else
                {
                    IpListDescription = "Нет доступных IP адресов";
                    DetectedHosts = null;
                    DetectedMacs = null;
                    IsVisible = "Hidden";
                }
            }
            catch (NullReferenceException ex)
            {
                ExceptionText = ex.Message;
            }

        }

        /// <summary>
        /// Метод, который выводит доступные IP адреса
        /// </summary>
        /// <param name="ipV4">IP адрес формата v4 текущего компьютера</param>
        /// <returns></returns>
        public ObservableCollection<IPs> CheckCurrentNetwork(string ipV4)
        {
            try
            {
                string[] startParts = StartAddress.Split('.');
                int startAddress = Convert.ToInt32(startParts[3]);
                string[] endParts = EndAddress.Split('.');
                int endAddress = Convert.ToInt32(endParts[3]);
                ObservableCollection<IPs> detectedAddresses = new ObservableCollection<IPs>();
                ObservableCollection<string> detectedHosts = new ObservableCollection<string>();
                ObservableCollection<string> detectedMacs = new ObservableCollection<string>();
                Ping ping = new Ping();

                for (int addressPart = startAddress; addressPart <= endAddress; addressPart++)
                {
                    string currentAddressAsString = $"{ipV4}{addressPart}";
                    IPAddress currentAddress = IPAddress.Parse(currentAddressAsString);
                    var pingResult = ping.Send(currentAddress, 700);
                    Debug.WriteLine($"[{DateTime.Now}] {currentAddress}: {pingResult.Status}");

                    if (pingResult.Status == IPStatus.Success)
                    {
                        IPs ipModel = new IPs { Ip = currentAddress.ToString(), FamilyIp = currentAddress.AddressFamily.ToString() };
                        detectedAddresses.Add(ipModel);
                        detectedHosts.Add(GetHostNameByIp(currentAddress.ToString()));
                        detectedMacs.Add(GetMacAddress(currentAddress.ToString()));
                    }
                }
                DetectedHosts = detectedHosts;
                DetectedMacs = detectedMacs;
                return detectedAddresses;
            }
            catch (Exception ex)
            {
                ExceptionText = ex.Message;
                return detectedAddresses;
            }

        }

        public void GetPorts(IPs ip)
        {
            var freePorts = GetFreePorts(ip.Ip);

            foreach (var port in freePorts)
            {
                Debug.WriteLine($"Порт {port} свободен для {ip.Ip}");
            }
        }

        static List<int> GetFreePorts(string ipAddress)
        {
            var freePorts = new List<int>();
            var ip = IPAddress.Parse(ipAddress);

            Parallel.For(5000, 5900, port =>
            {
                if (IsPortFree(ip, port))
                {
                    lock (freePorts)
                    {
                        freePorts.Add(port);
                    }
                }
            });
            return freePorts;
        }

        static bool IsPortFree(IPAddress ipAddress, int port)
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    var endPoint = new IPEndPoint(ipAddress, port);
                    var result = socket.BeginConnect(endPoint, null, null);
                    if (result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(700)))
                    {
                        socket.EndConnect(result);
                        Debug.WriteLine($"{ipAddress}:{port}");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при проверке порта {port}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Конструктор MainWindowViewModel
        /// </summary>
        /// <param name="navService">Навигация</param>
        public MainWindowViewModel(INavigationService navService)
        {

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
    }
}
