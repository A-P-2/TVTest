using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class UDPClient
    {
        private readonly string deviceName;
        private readonly Socket socket;
        private readonly EndPoint endPoint;

        public UDPClient(string deviceName, IPAddress address, int port)
        {
            this.deviceName = deviceName;   

            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

            endPoint = new IPEndPoint(address, port);
        }

        public void StartAddressReminderLoop(int millisecondsDelay = 1000)
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await SendAsync($"_{deviceName}");
                        await Task.Delay(millisecondsDelay);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
        }

        public void StartEventLoop(int millisecondsDelay = 1000)
        {
            Random random = new();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        Data data = new(deviceName, random.Next(1000), (float)random.Next(1000) / 100, "text" + random.Next(1000).ToString());
                        await SendAsync(data.ToJsonString()!);
                        await Task.Delay(millisecondsDelay);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
        }

        public async Task SendAsync(string data)
        {
            //Console.WriteLine($"Отправляю: {data}");
            var s = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await socket.SendToAsync(s, SocketFlags.None, endPoint);
        }
    }
}
