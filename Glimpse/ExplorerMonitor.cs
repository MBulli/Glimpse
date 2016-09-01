using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Glimpse
{
    class ExplorerMonitor
    {
        private const string ExplorerClassName = "CabinetWClass";
        private const string DesktopClassName = "Progman";

        private readonly int CurrentProcessID;
        private AutomationElement lastTopLevelWindow;

        public event EventHandler<ExplorerMonitorEventArgs> ExplorerWindowGotFocus;
        public event EventHandler<ExplorerMonitorEventArgs> ExplorerSelectionChanged;

        public ExplorerMonitor()
        {
            CurrentProcessID = Process.GetCurrentProcess().Id;
        }

        public void Start()
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        public void Stop()
        {
            Automation.RemoveAllEventHandlers();
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            try
            {
                AutomationElement elementFocused = sender as AutomationElement;

                if (IsInvalidAutomationElement(elementFocused))
                    return;

                AutomationElement newTopLevelWindow = GetTopLevelWindow(elementFocused);

                if (IsInvalidAutomationElement(newTopLevelWindow))
                    return;

                if (newTopLevelWindow != lastTopLevelWindow)
                {
                    if (IsExplorerWindow(lastTopLevelWindow))
                    {
                        RemoveSelectionEvenhandler(lastTopLevelWindow);
                    }

                    if (IsExplorerWindow(newTopLevelWindow))
                    {
                        Console.WriteLine("Focus moved to new top-level explorer window " + newTopLevelWindow.Current.Name);

                        AddSelectionEvenhandler(newTopLevelWindow);

                        ExplorerWindowGotFocus?.Invoke(this, new ExplorerMonitorEventArgs(new IntPtr(newTopLevelWindow.Current.NativeWindowHandle)));
                    }
                    else
                    {
                        Console.WriteLine("Focus moved to new top-level non-explorer window " + newTopLevelWindow.Current.Name);
                    }

                    lastTopLevelWindow = newTopLevelWindow;
                }
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine("SelectionMonitor: Fail OnFocusChanged " + ex.ToString());
            }
        }

        private void AddSelectionEvenhandler(AutomationElement element)
        {
            if (!IsInvalidAutomationElement(element))
            {
                TreeScope scope = TreeScope.Descendants;
                AutomationEventHandler handler = SelectionChanged;

                Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementSelectedEvent, element, scope, handler);
                Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent, element, scope, handler);
                Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent, element, scope, handler);
                //Automation.AddAutomationEventHandler(SelectionPatternIdentifiers.InvalidatedEvent, element, scope, handler);
            }
        }

        private void RemoveSelectionEvenhandler(AutomationElement element)
        {
            if (!IsInvalidAutomationElement(element))
            {
                AutomationEventHandler handler = SelectionChanged;

                Automation.RemoveAutomationEventHandler(SelectionItemPatternIdentifiers.ElementSelectedEvent, element, handler);
                Automation.RemoveAutomationEventHandler(SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent, element, handler);
                Automation.RemoveAutomationEventHandler(SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent, element, handler);
                //Automation.RemoveAutomationEventHandler(SelectionPatternIdentifiers.InvalidatedEvent, element, handler);
            }
        }

        private void SelectionChanged(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;

            if (IsInvalidAutomationElement(element))
                return;

            Console.WriteLine("Selection changed. Sender: " + element.Current.Name);
            ExplorerSelectionChanged?.Invoke(this, new ExplorerMonitorEventArgs(new IntPtr(lastTopLevelWindow.Current.NativeWindowHandle)));
        }

        /// <summary>
        /// Retrieves the top-level window that contains the specified 
        /// UI Automation element.
        /// </summary>
        private AutomationElement GetTopLevelWindow(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent;
            AutomationElement node = element;
            try  // In case the element disappears suddenly, as menu items are likely to do.
            {
                if (node == AutomationElement.RootElement)
                    return node;

                // Walk up the tree to the child of the root.
                while (true)
                {
                    elementParent = walker.GetParent(node);
                    if (elementParent == null)
                    {
                        return null;
                    }
                    if (elementParent == AutomationElement.RootElement)
                    {
                        break;
                    }
                    node = elementParent;
                }
            }
            catch (ElementNotAvailableException)
            {
                node = null;
            }
            catch (ArgumentNullException)
            {
                node = null;
            }
            return node;
        }

        private bool IsExplorerWindow(AutomationElement element)
        {
            try
            {
                if (IsInvalidAutomationElement(element))
                    return false;
                else
                    return element.Current.ClassName == ExplorerClassName;
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        private bool IsInvalidAutomationElement(AutomationElement element)
        {
            try
            {
                if (element == null)
                    return true;
                if (element.Current.ProcessId == CurrentProcessID)
                    return true;

                return false;
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }
    }

    class ExplorerMonitorEventArgs : EventArgs
    {
        public readonly IntPtr ExplorerWindowHandle;

        public ExplorerMonitorEventArgs(IntPtr hwnd)
        {
            ExplorerWindowHandle = hwnd;
        }
    }
}
