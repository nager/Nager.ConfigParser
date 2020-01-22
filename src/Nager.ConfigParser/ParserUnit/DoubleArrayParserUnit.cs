using System;
using System.Globalization;
using System.Linq;

namespace Nager.ConfigParser.ParserUnit
{
    internal class DoubleArrayParserUnit : BaseParserUnit
    {
        public override Type ParserUnitType => typeof(double[]);

        public override object Deserialize(string value)
        {
            var parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Select(o =>
            {
                var success = double.TryParse(o, NumberStyles.Number, CultureInfo.InvariantCulture, out double temp);
                return new { success, temp };
            })
            .Where(o => o.success)
            .Select(o => o.temp).ToArray();
        }

        public override string Serialize(object value)
        {
            if (value is double[] items)
            {
                return string.Join(",", items.Select(o => o.ToString("0.00", CultureInfo.InvariantCulture)));
            }

            return null;
        }
    }
}
