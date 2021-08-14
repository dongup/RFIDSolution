using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using RFIDSolution.Shared.Protos;
using Blazored.Modal;
using RFIDSolution.WebAdmin.Service;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace RFIDSolution.WebAdmin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var baseUri = "http://localhost:5000";
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });

            builder.Services.AddSingleton(services =>
            {
                httpClient.EnableIntercept(services);
                return new ShoeModelProto.ShoeModelProtoClient(channel);
            });
            builder.Services.AddSingleton(services =>
            {
                httpClient.EnableIntercept(services);
                return new RFTagProto.RFTagProtoClient(channel);
            });
            builder.Services.AddSingleton(services =>
            {
                httpClient.EnableIntercept(services);
                return new ProductProto.ProductProtoClient(channel);
            });

            builder.Services.AddBlazoredModal();
            builder.Services.AddTransient<DialogService>();
            builder.Services.AddLoadingBar(op => {
                op.LoadingBarColor = "midnightblue";
            });

            await builder.Build().UseLoadingBar().RunAsync();
        }
    }
}
