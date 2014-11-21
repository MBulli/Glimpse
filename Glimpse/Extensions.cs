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
            // substract non client area
            return new System.Windows.Size(wnd.Width 
                                            - System.Windows.SystemParameters.WindowNonClientFrameThickness.Left
                                            - System.Windows.SystemParameters.WindowNonClientFrameThickness.Right,
                                           wnd.Height
                                            - System.Windows.SystemParameters.WindowNonClientFrameThickness.Top
                                            - System.Windows.SystemParameters.WindowNonClientFrameThickness.Bottom);
        }

        public static void SetClientSize(this System.Windows.Window wnd, System.Windows.Size clientSize)
        {
            // add non client area to client size 
            wnd.Width = clientSize.Width
                        + System.Windows.SystemParameters.WindowNonClientFrameThickness.Left
                        + System.Windows.SystemParameters.WindowNonClientFrameThickness.Right;

            wnd.Height = clientSize.Height
                         + System.Windows.SystemParameters.WindowNonClientFrameThickness.Top
                         + System.Windows.SystemParameters.WindowNonClientFrameThickness.Bottom;
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
