using System;

namespace Nager.DotConfigParser
{
    public abstract class BaseParserUnit
    {
        public abstract Type ParserUnitType { get; }

        public abstract object Deserialize(string value);

        public abstract string Serialize(object value);
    }
}
