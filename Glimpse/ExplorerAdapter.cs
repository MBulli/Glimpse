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
        private dynamic shell;

        public ExplorerAdapter()
        {
            Type typeShell = Type.GetTypeFromProgID("Shell.Application");
            this.shell = Activator.CreateInstance(typeShell);
        }

        public string[] GetSelectedItems(IntPtr target)
        {
            // see:
            // http://blogs.msdn.com/b/oldnewthing/archive/2013/04/22/10412906.aspx
            // http://blogs.msdn.com/b/oldnewthing/archive/2004/07/20/188696.aspx

            dynamic shellWindows = shell.Windows(); // IShellWindows

            for (var i = 0; i < shellWindows.Count; i++)
            {
                dynamic w = shellWindows.Item(i);  // IWebBrowserApp
                //Console.WriteLine(w.LocationName);

                long hwnd = w.HWND;
                if (hwnd == target.ToInt64())
                {
                    dynamic selection = w.Document.SelectedItems();

                    string[] items = null;
                    if (selection != null)
                    {
                        items = new string[selection.Count];
                        for (var j = 0; j < selection.Count; j++)
                        {
                            dynamic item = selection.Item(j);
                            //Console.WriteLine(item.Name);
                            //Console.WriteLine(item.Path);

                            items[j] = item.Path as string;
                        }
                    }

                    return items;
                }
            }

            return null;
        }  
    }
}