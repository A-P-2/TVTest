using Server;
using Server.DataController;

DatabaseEmulator databaseEmulator = new();
if (!databaseEmulator.ConnectToDB("temp"))
{
    Console.WriteLine("Не удалось подключиться к БД!");
    return;
}

var server = new UDPServer(databaseEmulator);
server.StartRegistrationLoop(1000);
server.StartEventLoop(1000);
server.StartMessageLoop();

string? command;
while ((command = Console.ReadLine()) != "exit")
{
    if (command == "show") server.PrintEventList(Console.WriteLine);
}
