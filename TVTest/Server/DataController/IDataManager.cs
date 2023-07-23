namespace Server.DataController
{
    interface IDataManager
    {
        public void AddDevice(string deviceName, string deviceIPaddress);
        public void AddEvent(string deviceName, int someInt, float someFloat, string someStr);
        public void PrintEventList(Action<string> tableRowHandler);
    }
}
