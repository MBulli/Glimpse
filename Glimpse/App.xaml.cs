using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Glimpse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SingleInstanceApplication singleInstanceApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            this.singleInstanceApp = new SingleInstanceApplication();

            if (singleInstanceApp.IsMasterInstanceRunning())
            {
                singleInstanceApp.RunAsSlave(e.Args);
                this.Shutdown();
                return;
            }

            singleInstanceApp.RunAsMaster();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            singleInstanceApp.StopServer();
            System.Windows.Automation.Automation.RemoveAllEventHandlers();
        }
    }
}
