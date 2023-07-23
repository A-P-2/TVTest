using System.Text.Json;

namespace Client
{
    class Data
    {
        public string DeviceName { get; private set; } = "";
        public int SomeInt { get; private set; } = 0;
        public float SomeFloat { get; private set; } = 0f;
        public string SomeStr { get; private set; } = "";

        public Data(string deviceName, int someInt, float someFloat, string someStr)
        {
            DeviceName = deviceName;
            SomeInt = someInt;
            SomeFloat = someFloat;
            SomeStr = someStr;
        }

        public string ToJsonString() => JsonSerializer.Serialize(this);
        static public Data? ToDataObject(string jsonString) => JsonSerializer.Deserialize<Data>(jsonString);
    }
}
