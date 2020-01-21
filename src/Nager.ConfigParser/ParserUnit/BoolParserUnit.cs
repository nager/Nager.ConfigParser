using System;

namespace Nager.ConfigParser.ParserUnit
{
    internal class BoolParserUnit : BaseParserUnit
    {
        public override Type ParserUnitType => typeof(bool);

        public override object Deserialize(string value)
        {
            bool.TryParse(value, out var result);
            return result;
        }

        public override string Serialize(object value)
        {
            return value.ToString();
        }
    }
}
