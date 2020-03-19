using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using BlazorWebView;
using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Blazor.Gitter.Core.Components.Shared;
using System.Net.Http;

namespace Blazor.Gitter.WindowsApp
{
    public class Startup
    {
        /// <summary>
        /// Configures and adds services to the servicecollection.
        /// </summary>
        /// <param name="services">The collection of services to add to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();

            services.AddSingleton<HttpClient>(x => new HttpClient())
                .AddSingleton<IChatApi>(s => new GitterApi(s.GetRequiredService<HttpClient>(), true))
                .AddSingleton<ILocalStorageService, WindowsLocalStorageService>()
                .AddSingleton<ILocalisationHelper, LocalisationHelper>()
                .AddSingleton<IAppState, AppState>();
        }

        /// <summary>
        /// Configure the app.
        /// </summary>
        /// <param name="app">The application builder for apps.</param>
        public void Configure(ApplicationBuilder app)
        {
            app.AddComponent<Core.Components.App>("app");
        }
    }
}