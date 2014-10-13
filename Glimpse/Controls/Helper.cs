using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Controls
{
    class Helper
    {
        public static string FormattedByteSize(long x, bool showDecimalPlaces = true)
        {
            float value;
            string unit;

            if (x >= 1024 * 1024 * 1024)
            {
                value = x / (1024 * 1024 * 1024.0f);
                unit = "GB";
            }
            else if (x >= 1024 * 1024)
            {
                value = x / (1024 * 1024.0f);
                unit = "MB";
            }
            else if (x >= 1024)
            {
                value = x / 1024.0f;
                unit = "KB";
            }
            else
            {
                value = x;
                unit = "Bytes";
            }

            if (showDecimalPlaces)
                return string.Format("{0:0.##} {1}", value, unit);
            else
                return string.Format("{0} {1}", (int)value, unit);
        }
    }
}
