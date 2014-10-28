using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

using Rect = System.Windows.Rect;
using Window = System.Windows.Window;

namespace Glimpse.Models
{
    class Screen
    {
        public bool IsPrimary
        {
            get;
            private set;
        }

        public Rect Bounds
        {
            get;
            private set;
        }

        public Rect WorkingArea
        {
            get;
            private set;
        }

        public static Screen FromMainWindow()
        {
            return FromWindow(System.Windows.Application.Current.MainWindow);
        }

        public static Screen FromWindow(Window wnd)
        {
            if (wnd == null)
                throw new ArgumentNullException("wnd");

            var hwnd = new WindowInteropHelper(wnd);
            var hMonitor = Interop.NativeMethods.MonitorFromWindow(hwnd.Handle, Interop.NativeMethods.MONITOR_DEFAULTTONEAREST);

            var mi = new Interop.NativeMethods.MONITORINFOEX();
            Interop.NativeMethods.GetMonitorInfo(hMonitor, mi);

            Rect bounds = new Rect(mi.rcMonitor.left,
                                   mi.rcMonitor.top,
                                   mi.rcMonitor.right - mi.rcMonitor.left,
                                   mi.rcMonitor.bottom - mi.rcMonitor.top);

            Rect workArea = new Rect(mi.rcWork.left,
                                     mi.rcWork.top,
                                     mi.rcWork.right - mi.rcWork.left,
                                     mi.rcWork.bottom - mi.rcWork.top);

            bool primary = (mi.dwFlags & Interop.NativeMethods.MONITORINFOF_PRIMARY) != 0;

            return new Screen() { Bounds = bounds, WorkingArea = workArea, IsPrimary = primary };
        }
    }
}
