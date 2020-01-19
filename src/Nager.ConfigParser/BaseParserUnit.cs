using System;

namespace Nager.ConfigParser
{
    public abstract class BaseParserUnit
    {
        public abstract Type ParserUnitType { get; }

        public abstract object Deserialize(string value);

        public abstract string Serialize(object value);
    }
}
