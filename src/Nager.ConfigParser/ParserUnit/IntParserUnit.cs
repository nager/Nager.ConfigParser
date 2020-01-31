using System;

namespace Nager.ConfigParser.ParserUnit
{
    internal class IntParserUnit : BaseParserUnit
    {
        public override Type ParserUnitType => typeof(int);

        public override object Deserialize(string value)
        {
            if (int.TryParse(value, out var temp))
            {
                return temp;
            }

            return 0;
        }

        public override string Serialize(object value)
        {
            if (value == null)
            {
                return null;
            }

            return $"{value}";
        }
    }
}
