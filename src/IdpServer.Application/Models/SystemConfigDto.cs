namespace IdpServer.Application.Model
{
    public class SystemConfigDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public SystemConfigDto(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
