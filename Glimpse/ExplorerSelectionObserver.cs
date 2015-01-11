using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Glimpse
{
    class ExplorerSelectionObserver
    {
        private const string ExplorerClassName = "CabinetWClass";
        private const string DesktopClassName = "Progman";
        private static readonly int CurrentProcessID;

        public event EventHandler<string[]> ExplorerSelectionChanged;

        static ExplorerSelectionObserver()
        {
            CurrentProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        public void StartObserver()
        {
            Automation.AddAutomationEventHandler(WindowPatternIdentifiers.WindowOpenedEvent,
                                                 AutomationElement.RootElement,
                                                 TreeScope.Descendants,
                                                 WindowOpend);

            foreach (AutomationElement explorer in FindAllExplorerWindows())
            {
                AddSelectionEvenhandler(explorer);
            }
        }

        public void StopObserver()
        {
            Automation.RemoveAllEventHandlers();
        }

        private void WindowOpend(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;

            if (IsInvalidAutomationElement(element))
                return;

            if (element.Current.ClassName == ExplorerClassName)
            {
                AddSelectionEvenhandler(element);
            }
        }

        private void SelectionChanged(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;

            if (IsInvalidAutomationElement(element))
                return;

            AutomationElement parent = GetExplorerWindow(element);

            if (parent != null)
            {
                IntPtr hwnd = new IntPtr(parent.Current.NativeWindowHandle);

                string[] items = ExplorerAdapter.GetSelectedItems(hwnd);

                if (items != null)
                {
                    if (ExplorerSelectionChanged != null)
                    {
                        ExplorerSelectionChanged(this, items);
                    }                    
                }
            }
        }

        private IEnumerable<AutomationElement> FindAllExplorerWindows()
        {
            var cond = new OrCondition(new PropertyCondition(AutomationElement.ClassNameProperty, ExplorerClassName),
                                       new PropertyCondition(AutomationElement.ClassNameProperty, DesktopClassName));

            return AutomationElement.RootElement.FindAll(TreeScope.Children, cond)
                                                .Cast<AutomationElement>()
                                                .Where(e => e.Current.ProcessId != CurrentProcessID);
        }

        private void AddSelectionEvenhandler(AutomationElement element)
        {
            TreeScope scope = TreeScope.Descendants;
            AutomationEventHandler handler = SelectionChanged;

            Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementSelectedEvent, element, scope, handler);
            Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent, element, scope, handler);
            Automation.AddAutomationEventHandler(SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent, element, scope, handler);
            Automation.AddAutomationEventHandler(SelectionPatternIdentifiers.InvalidatedEvent, element, scope, handler);
        }

        private AutomationElement GetExplorerWindow(AutomationElement element)
        {
            AutomationElement parent = element;

            while (true)
            {
                parent = TreeWalker.RawViewWalker.GetParent(parent);

                if (parent == AutomationElement.RootElement)
                    return null;
                if (parent.Current.ClassName == ExplorerClassName
                    || parent.Current.ClassName == DesktopClassName)
                {
                    return parent;
                }
            }
        }

        private bool IsInvalidAutomationElement(AutomationElement element)
        {
            if (element == null)
                return true;
            if (element.Current.ProcessId == CurrentProcessID)
                return true;

            return false;
        }
    }
}
