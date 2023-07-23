namespace Server.DataController
{
    class DatabaseEmulator : IDataManager
    {
        private class DeviceTableRow
        {
            public string deviceName;
            public string deviceIPaddress;

            public DeviceTableRow(string deviceName, string deviceIPaddress)
            {
                this.deviceName = deviceName;
                this.deviceIPaddress = deviceIPaddress;
            }
        }

        private class EventTableRow
        {
            public string deviceName;
            public int someInt;
            public float someFloat;
            public string someStr;

            public EventTableRow(string deviceName, int someInt, float someFloat, string someStr)
            {
                this.deviceName = deviceName;
                this.someInt = someInt;
                this.someFloat = someFloat;
                this.someStr = someStr;
            }
        }

        private readonly List<DeviceTableRow> deviceTable = new();
        private readonly List<EventTableRow> eventTable = new();

        public bool ConnectToDB(string connectionString) => true;
        /*
        В данном случае функция ConnectToDB работает чисто как заглушка, для демонстариции работы прототипа сервера.
        При использовании реальной БД здесь бы происходило подключение к БД, 
        а также инициализация БД со всеми таблицами в случае необходимости
        */

        public void AddDevice(string deviceName, string deviceIPaddress)
        {
            foreach (var row in deviceTable)
            {
                if (row.deviceName == deviceName)
                {
                    row.deviceIPaddress = deviceIPaddress;
                    return;
                }
            }

            deviceTable.Add(new DeviceTableRow(deviceName, deviceIPaddress));
        }

        public void AddEvent(string deviceName, int someInt, float someFloat, string someStr)
        {
            eventTable.Add(new EventTableRow(deviceName, someInt, someFloat, someStr));
        }

        public void PrintEventList(Action<string> tableRowHandler)
        {
            foreach (var row in eventTable)
            {
                string? eventIP = deviceTable.Find((x) => x.deviceName == row.deviceName)?.deviceIPaddress;
                eventIP ??= "???";

                string rowString = $"{row.deviceName}\t|\t{eventIP}\t|\t{row.someInt}\t|\t{row.someFloat}\t|\t{row.someStr}";
                tableRowHandler(rowString);
            }
        }
    }
}
