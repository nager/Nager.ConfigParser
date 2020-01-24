using System.Globalization;

namespace Nager.ConfigParser
{
    public class ConfigConvertConfig
    {
        /// <summary>
        /// The delimiters of a full config for example a new line
        /// </summary>
        public char[] ConfigDelimiter { get; set; }
        /// <summary>
        /// The delimiter char between key and value
        /// </summary>
        public char KeyValueDelimiter { get; set; }
        /// <summary>
        /// The delimiter of array data in the value
        /// </summary>
        public char ValueDelimiter { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public ConfigConvertConfig()
        {
            this.ConfigDelimiter = new char[] { '\r', '\n' };
            this.KeyValueDelimiter = '=';
            this.ValueDelimiter = ',';
            this.CultureInfo = CultureInfo.InvariantCulture;
        }
    }
}
