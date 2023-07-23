using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.DataController;

namespace Server
{
    class UDPServer
    {
        private readonly int port = 5000;

        private readonly Socket socket;
        private readonly EndPoint endPoint;

        private readonly byte[] buffer;
        private readonly ArraySegment<byte> bufferSegment;

        private readonly IDataManager dataManager;

        private readonly Queue<Tuple<string, EndPoint>> registrationQueue = new();
        private readonly Queue<Data> eventQueue = new();

        public UDPServer(IDataManager dataManager)
        {
            buffer = new byte[1024];
            bufferSegment = new(buffer);

            endPoint = new IPEndPoint(IPAddress.Any, port);

            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            socket.Bind(endPoint);

            this.dataManager = dataManager;
        }

        public void StartMessageLoop()
        {
            _ = Task.Run(async () =>
            {
                SocketReceiveMessageFromResult result;
                while (true)
                {
                    try
                    {
                        result = await socket.ReceiveMessageFromAsync(bufferSegment, SocketFlags.None, endPoint);
                        string requestString = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);
                        //Console.WriteLine($"Получил: {requestString}");

                        if (requestString[0].Equals('_')) registrationQueue.Enqueue(new Tuple<string, EndPoint>(requestString[1..], result.RemoteEndPoint));
                        else eventQueue.Enqueue(Data.ToDataObject(requestString)!);
                    } 
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
        }

        public void StartRegistrationLoop(int millisecondsDelay = 1000)
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        while (registrationQueue.Count == 0) await Task.Delay(millisecondsDelay);
                        while (registrationQueue.Count > 0)
                        {
                            var registrationData = registrationQueue.Dequeue();
                            dataManager.AddDevice(registrationData.Item1, registrationData.Item2.ToString()!.Split(':')[0]);
                        }
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
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        while (eventQueue.Count == 0) await Task.Delay(millisecondsDelay);
                        while (eventQueue.Count > 0)
                        {
                            var eventData = eventQueue.Dequeue();
                            dataManager.AddEvent(eventData.DeviceName, eventData.SomeInt, eventData.SomeFloat, eventData.SomeStr);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
        }

        public void PrintEventList(Action<string> tableRowHandler) => dataManager.PrintEventList(tableRowHandler);
    }
}
