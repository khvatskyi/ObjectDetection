using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;
using BlazorUI.Mapping;
using BlazorUI.Repository;

namespace BlazorUI
{
    public class Program
    {
        private const string ApiAddress = "https://localhost:44377/";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ApiAddress) });
            builder.Services.AddSingleton(serviceProvider => (IJSInProcessRuntime)serviceProvider.GetRequiredService<IJSRuntime>());
            builder.Services.AddScoped<IImageHttpRepository, ImageHttpRepository>();
            builder.Services.AddFileReaderService(o => o.UseWasmSharedBuffer = true);
            builder.Services.AddAutoMapper(typeof(ObjectResultProfile).Assembly);

            await builder.Build().RunAsync();
        }
    }
}
