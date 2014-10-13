using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse
{
    class ExplorerAdapter
    {
        public static string GetSelectedItem(IntPtr target)
        {
            // see:
            // http://blogs.msdn.com/b/oldnewthing/archive/2013/04/22/10412906.aspx
            // http://blogs.msdn.com/b/oldnewthing/archive/2004/07/20/188696.aspx

            Type typeShell = Type.GetTypeFromProgID("Shell.Application");
            dynamic shell = Activator.CreateInstance(typeShell);
            dynamic shellWindows = shell.Windows(); // IShellWindows

            for (var i = 0; i < shellWindows.Count; i++)
            {
                dynamic w = shellWindows.Item(i);  // IWebBrowserApp
                //Console.WriteLine(w.LocationName);

                long hwnd = w.HWND;
                if (hwnd == target.ToInt64())
                {
                    dynamic selection = w.Document.SelectedItems();

                    for (var j = 0; j < selection.Count; j++)
                    {
                        dynamic item = selection.Item(j);
                        Console.WriteLine(item.Name);
                        Console.WriteLine(item.Path);

                        return item.Path as string;
                    }                    
                }
            }

            return null;
        }       
    }
}