using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Services
{
    public class ClientService 
    {
        public async void GetData()
        {
            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 0);
            byte[] data = new byte[512];
            var stream = tcpClient.GetStream();
            int bytes = await stream.ReadAsync(data);
            string time = Encoding.UTF8.GetString(data, 0, bytes);
            Debug.WriteLine($"Текущее время: {time}");
        }
    }
}
