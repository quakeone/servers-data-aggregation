namespace ServersDataAggregation.Common.Model
{
    public class ServerSetting
    {
        public string Setting { get; internal set; }
        public string Value { get; internal set; }

        public ServerSetting(string pSetting, string pValue)
        {
            Setting = pSetting;
            Value = pValue;
        }
    }
}
