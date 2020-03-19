using BlazorWebView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Blazor.Gitter.WindowsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// disposable usable to stop blazor..
        /// </summary>
        private IDisposable run;

        /// <summary>
        /// Bool signaling whether the application is initialized.
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raises the ContentRendered event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!this.initialized)
            {
                this.initialized = true;
                this.run = BlazorWebViewHost.Run<Startup>(this.BlazorWebView, "wwwroot/index.html");
            }
        }
    }
}
