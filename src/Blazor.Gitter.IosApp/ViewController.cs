using BlazorWebView;
using Foundation;
using System;
using UIKit;

namespace Blazor.Gitter.IosApp
{
    public partial class ViewController : UIViewController
    {
        private IDisposable disposable;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            this.disposable = BlazorWebViewHost.Run<Startup>(this.BlazorWebView, "wwwroot/index.html");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
            this.disposable.Dispose();
        }
    }
}