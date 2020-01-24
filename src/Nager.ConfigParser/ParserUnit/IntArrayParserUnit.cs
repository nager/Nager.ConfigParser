using System;
using System.Linq;

namespace Nager.ConfigParser.ParserUnit
{
    internal class IntArrayParserUnit : BaseParserUnit
    {
        public IntArrayParserUnit(char valueDelimiter) : base(valueDelimiter)
        { }

        public override Type ParserUnitType => typeof(int[]);

        public override object Deserialize(string value)
        {
            var parts = value.Split(new char[] { base._valueDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Select(o =>
            {
                var success = int.TryParse(o, out int temp);
                return new { success, temp };
            })
            .Where(o => o.success)
            .Select(o => o.temp).ToArray();
        }

        public override string Serialize(object value)
        {
            if (value is int[] items)
            {
                return string.Join(base._valueDelimiter.ToString(), items);
            }

            return null;
        }
    }
}
