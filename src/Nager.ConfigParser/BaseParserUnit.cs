using System;
using System.Globalization;

namespace Nager.ConfigParser
{
    public abstract class BaseParserUnit
    {
        internal readonly CultureInfo _cultureInfo;
        internal readonly char _delimiterChar;

        public BaseParserUnit()
        { }

        public BaseParserUnit(CultureInfo cultureInfo)
        {
            this._cultureInfo = cultureInfo;
        }

        public BaseParserUnit(CultureInfo cultureInfo, char delimiterChar)
        {
            this._cultureInfo = cultureInfo;
            this._delimiterChar = delimiterChar;
        }

        public BaseParserUnit(char delimiterChar)
        {
            this._delimiterChar = delimiterChar;
        }

        public abstract Type ParserUnitType { get; }

        public abstract object Deserialize(string value);

        public abstract string Serialize(object value);
    }
}
