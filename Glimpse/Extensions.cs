using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse
{
    static class Extensions
    {
        public static string[] Split(this string str, string seperator)
        {
            return str.Split(new[] { seperator }, StringSplitOptions.None);
        }
    }
}
