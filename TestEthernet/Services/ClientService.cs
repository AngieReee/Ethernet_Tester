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

        bool connectionStatus;
        public bool ConnectionStatus { get => connectionStatus;
            set
            { 
                connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
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
                ConnectionStatus = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ConnectionStatus = false;
            }
        }

        public async void Send(string text)
        {
            try
            {
                var message = text;
                var data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);

                var buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var checkData = buffer.Take(bytesRead - 2).ToArray();
                var receivedCrc = BitConverter.ToUInt16(buffer, bytesRead - 2);

                if (Crc16.Calculate(data) == receivedCrc)
                {
                    Debug.WriteLine("Данные не повреждены.");
                }
                Debug.WriteLine($"Вы: {message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
