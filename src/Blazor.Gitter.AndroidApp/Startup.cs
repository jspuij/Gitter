using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using BlazorWebView;
using Blazor.Gitter.Core.Components;
using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Blazor.Gitter.Core.Components.Shared;
using System.Net.Http;

namespace Blazor.Gitter.AndroidApp
{
    public class Startup
    {
        /// <summary>
        /// Configures and adds services to the servicecollection.
        /// </summary>
        /// <param name="services">The collection of services to add to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<HttpClient>()
                .AddSingleton<IChatApi, GitterApi>()
                .AddSingleton<ILocalStorageService, XamarinLocalStorageService>()
                .AddSingleton<ILocalisationHelper, LocalisationHelper>()
                .AddSingleton<IAppState, AppState>();
        }

        /// <summary>
        /// Configure the app.
        /// </summary>
        /// <param name="app">The application builder for apps.</param>
        public void Configure(ApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}