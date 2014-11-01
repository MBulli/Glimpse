using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Views.Converter
{
    abstract class BaseConverter : System.Windows.Markup.MarkupExtension
    {
        public BaseConverter()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
