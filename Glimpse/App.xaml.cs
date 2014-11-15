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
            singleInstanceApp = new SingleInstanceApplication();

            if (!singleInstanceApp.IsMasterInstanceRunning())
            {
                singleInstanceApp.RunAsMaster((args) =>
                {
                    // TODO handle args
                });
            }
            else
            {
                singleInstanceApp.RunAsSlave(e.Args);
                this.Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
