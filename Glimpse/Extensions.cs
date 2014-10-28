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

        public static System.Windows.Size GetClientSize(this System.Windows.Window wnd)
        {
            return new System.Windows.Size(((System.Windows.FrameworkElement)wnd.Content).ActualWidth,
                                           ((System.Windows.FrameworkElement)wnd.Content).ActualHeight);
        }

        public static System.Windows.Rect GetBounds(this System.Windows.Window wnd)
        {
            return new System.Windows.Rect(wnd.Left, wnd.Top, wnd.Width, wnd.Height);
        }

        public static void SetBounds(this System.Windows.Window wnd, System.Windows.Rect bounds)
        {
            wnd.Left = bounds.Left;
            wnd.Top = bounds.Top;
            wnd.Width = bounds.Width;
            wnd.Height = bounds.Height;
        }
    }
}
