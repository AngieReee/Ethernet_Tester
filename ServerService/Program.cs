using ServerService.Services;
using System.Diagnostics;
namespace ServerService
{
    internal class Program
    {
        static async Task Main()
        {
            var server = new NetworkServerService();
            await server.StartAsync(8080);
            Debug.WriteLine("Сервер запущен. Нажмите любую клавишу для остановки...");
        }
    }
}
