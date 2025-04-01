using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using ServerService.Models;
using System.Diagnostics;

namespace ServerService.Services
{
    public class NetworkServerService
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;

        public async Task StartAsync(int port)
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            while (!_cts.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = ProcessClientAsync(client);
            }
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                var buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                var data = buffer.Take(bytesRead - 2).ToArray();
                var receivedCrc = BitConverter.ToUInt16(buffer, bytesRead - 2);

                if (Crc16.Calculate(data) == receivedCrc)
                {
                    Debug.WriteLine("Данные не повреждены.");
                }
            }
        }
    }
}
