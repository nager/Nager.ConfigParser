namespace Nager.ConfigParser
{
    internal class Configuration
    {
        public string Key { get; set; }
        public string Data { get; set; }

        public Configuration(string key, string data)
        {
            this.Key = key.Trim();
            this.Data = data.Trim();
        }

        public override string ToString()
        {
            return $"{this.Key}={this.Data}";
        }
    }
}
