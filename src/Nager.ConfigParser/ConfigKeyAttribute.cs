using System;

namespace Nager.ConfigParser
{
    public class ConfigKeyAttribute : Attribute
    {
        public string Key { get; private set; }

        public ConfigKeyAttribute(string key)
        {
            this.Key = key;
        }
    }
}
