﻿using System;
using System.Globalization;
using System.Linq;

namespace Nager.ConfigParser.ParserUnit
{
    internal class DoubleArrayParserUnit : BaseParserUnit
    {
        public DoubleArrayParserUnit(CultureInfo cultureInfo, char delimiterChar) : base(cultureInfo, delimiterChar)
        { }

        public override Type ParserUnitType => typeof(double[]);

        public override object Deserialize(string value)
        {
            var parts = value.Split(new char[] { base._delimiterChar }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Select(o =>
            {
                var success = double.TryParse(o, NumberStyles.Number, base._cultureInfo, out double temp);
                return new { success, temp };
            })
            .Where(o => o.success)
            .Select(o => o.temp).ToArray();
        }

        public override string Serialize(object value)
        {
            if (value is double[] items)
            {
                return string.Join(base._delimiterChar.ToString(), items.Select(o => o.ToString(base._cultureInfo)));
            }

            return null;
        }
    }
}
