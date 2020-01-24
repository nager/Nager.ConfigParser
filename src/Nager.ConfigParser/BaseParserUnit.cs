using System;
using System.Globalization;

namespace Nager.ConfigParser
{
    public abstract class BaseParserUnit
    {
        internal readonly CultureInfo _cultureInfo;
        internal readonly char _valueDelimiter;

        public BaseParserUnit()
        { }

        public BaseParserUnit(CultureInfo cultureInfo)
        {
            this._cultureInfo = cultureInfo;
        }

        public BaseParserUnit(CultureInfo cultureInfo, char valueDelimiter)
        {
            this._cultureInfo = cultureInfo;
            this._valueDelimiter = valueDelimiter;
        }

        public BaseParserUnit(char valueDelimiter)
        {
            this._valueDelimiter = valueDelimiter;
        }

        public abstract Type ParserUnitType { get; }

        public abstract object Deserialize(string value);

        public abstract string Serialize(object value);
    }
}
