using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BlazorWebView;
using BlazorWebView.Android;

namespace Blazor.Gitter.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IBlazorWebView blazorWebView;

        private IDisposable disposable;

        /// <summary>
        /// Executes when permissions are requested.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="permissions">The requested permissions.</param>
        /// <param name="grantResults">The grant results.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] global::Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Executes when the activity is created.
        /// </summary>
        /// <param name="savedInstanceState">Optional saved state in case the actvity is resumed.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.activity_main);
            //this.SupportActionBar.Hide();

            this.blazorWebView = (IBlazorWebView)this.SupportFragmentManager.FindFragmentById(Resource.Id.blazorWebView);
            // run blazor.
            this.disposable = BlazorWebViewHost.Run<Startup>(this.blazorWebView, "wwwroot/index.html", new AndroidAssetResolver(this.Assets, "wwwroot/index.html").Resolve);
        }

        /// <summary>
        /// Perform any final cleanup before the activity is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            this.disposable.Dispose();
            base.OnDestroy();
        }
	}
}

