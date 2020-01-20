using System;

namespace Nager.ConfigParser.ParserUnit
{
    internal class DoubleParserUnit : BaseParserUnit
    {
        public override Type ParserUnitType => typeof(double);

        public override object Deserialize(string value)
        {
            if (!double.TryParse(value, out var temp))
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
