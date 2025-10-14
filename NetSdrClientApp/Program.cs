using System;
using System.Threading.Tasks;
using NetSdrClientApp;
using NetSdrClientApp.Networking;

/// <summary>
/// Програма для керування мережевим SDR-клієнтом.
/// Команди користувача:
/// C - підключення до сервера
/// D - відключення
/// F - зміна частоти
/// S - запуск/зупинка прослуховування IQ-потоку
/// Q - вихід
/// </summary>
internal static class Program
{
    private const string DefaultIp = "127.0.0.1";
    private const int DefaultPort = 5000;
    private const int DefaultUdpPort = 60000;
    private const int DefaultFrequency = 20_000_000;

    private static async Task Main()
    {
        Console.WriteLine(@"Usage:
C - connect
D - disconnect
F - set frequency
S - start/stop IQ listener
Q - quit");

        var tcpClient = new TcpClientWrapper(DefaultIp, DefaultPort);
        var udpClient = new UdpClientWrapper(DefaultUdpPort);
        var netSdr = new NetSdrClient(tcpClient, udpClient);

        while (true)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.C:
                    await netSdr.ConnectAsync();
                    break;

                case ConsoleKey.D:
                    netSdr.Disconect();
                    break;

                case ConsoleKey.F:
                    await netSdr.ChangeFrequencyAsync(DefaultFrequency, 1);
                    break;

                case ConsoleKey.S:
                    if (netSdr.IQStarted)
                        await netSdr.StopIQAsync();
                    else
                        await netSdr.StartIQAsync();
                    break;

                case ConsoleKey.Q:
                    Console.WriteLine("\nExiting program...");
                    return;

                default:
                    Console.WriteLine("\nUnknown command. Please use C, D, F, S, or Q.");
                    break;
            }
        }
    }
}
