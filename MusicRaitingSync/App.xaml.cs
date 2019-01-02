using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicRaitingSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] args;

        //~App() // bij afsluiten
        //{
        //    Debugger.Break();
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            args = e.Args;
        }

    }
}
