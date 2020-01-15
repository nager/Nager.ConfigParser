using System;

namespace Nager.DotConfigParser
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
