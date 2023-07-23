using Client;
using System.Net;

Console.Write("Введите название устройства: ");
string deviceName = Console.ReadLine()!;

var client = new UDPClient(deviceName, IPAddress.Loopback, 5000);
client.StartAddressReminderLoop(1000 * 60 * 5); // Напоминаем серверу IP адрес клиента каждые 5 минут
client.StartEventLoop(1000);

while (Console.ReadLine() != "exit") ;
