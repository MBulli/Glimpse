using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Glimpse.Views.Converter
{
    class ByteCountConverter : BaseConverter
    {
        public ByteCountConverter()
        {
        }

        public bool RoundToInteger
        {
            get; set;
        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string) && targetType != typeof(object))
                return null;

            long? byteValue = value as long?;
            if (byteValue == null)
                return null;

            long x = byteValue.Value;
            float result;
            string unit;

            if (x >= 1024 * 1024 * 1024)
            {
                result = x / (1024 * 1024 * 1024.0f);
                unit = "GB";
            }
            else if (x >= 1024 * 1024)
            {
                result = x / (1024 * 1024.0f);
                unit = "MB";
            }
            else if (x >= 1024)
            {
                result = x / 1024.0f;
                unit = "KB";
            }
            else
            {
                result = x;
                unit = "Bytes";
            }

            var ci = culture;
            if (RoundToInteger)
                return string.Format(ci, "{0:N0} {1}", (int)Math.Round(result), unit);
            else
                return string.Format(ci, "{0:N2} {1}", result, unit);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
