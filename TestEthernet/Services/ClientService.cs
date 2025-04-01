using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestEthernet.Models;
using TestEthernet.Core;
using System.Runtime.CompilerServices;

namespace TestEthernet.Services
{
    public class ClientService : ViewModel
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;

        // Свойство с уведомлением об изменении
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        // Метод для подключения
        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(IPAddress.Parse(ip), port);
            _stream = _client.GetStream();
            IsConnected = true; // Обновляем состояние подключения
        }

        // Метод для отправки данных
        public async Task SendDataAsync(byte[] data)
        {
            if (!IsConnected) throw new InvalidOperationException("Не подключено");

            var crc = Crc16.Calculate(data);
            var packet = data.Concat(BitConverter.GetBytes(crc)).ToArray();

            await _stream.WriteAsync(packet, 0, packet.Length);
        }

        // Метод для отключения
        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            IsConnected = false; // Обновляем состояние подключения
        }
    }
}
