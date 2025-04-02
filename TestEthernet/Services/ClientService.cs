using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TestEthernet.Models;
using TestEthernet.Core;
using System.Windows;

namespace TestEthernet.Services
{
    public class ClientService : ViewModel
    {
        TcpClient client;
        public TcpClient Client
        {
            get => client;
            set
            {
                client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        NetworkStream stream;
        public NetworkStream Stream { get => stream;
            set
            {
                stream = value;
                OnPropertyChanged(nameof(Stream));
            }
        }

        public async void Connect(string ip)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse(ip), 8080);
                stream = client.GetStream();
                Debug.WriteLine("Подключено");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async void Send(string text)
        {
            try
            {
                var message = text;
                var data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Debug.WriteLine($"Вы: {message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
