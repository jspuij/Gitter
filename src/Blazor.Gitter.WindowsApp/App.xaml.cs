using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Blazor.Gitter.WindowsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the application exit event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            System.Environment.Exit(0);
        }
    }
}
