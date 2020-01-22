using System;
using System.Globalization;

namespace Nager.ConfigParser.ParserUnit
{
    internal class DoubleParserUnit : BaseParserUnit
    {
        public DoubleParserUnit(CultureInfo cultureInfo) : base(cultureInfo)
        { }

        public override Type ParserUnitType => typeof(double);

        public override object Deserialize(string value)
        {
            if (!double.TryParse(value, NumberStyles.Number, base._cultureInfo, out var temp))
            {
                return temp;
            }

            return 0;
        }

        public override string Serialize(object value)
        {
            if (value is double item)
            {
                return item.ToString(base._cultureInfo);
            }

            return null;
        }
    }
}
