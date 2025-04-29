using EmbedIO;
using EmbedIO.WebApi;
using P2PPublicReverseProxy.Controllers;
using P2PPublicReverseProxy.Storage;
using System;
using System.Text;
using Bybit.P2P;

namespace P2PPublicReverseProxy
{
    internal class Program
    {
        public static Config cfg = Config.Load("cfg.json");
        public static Database db = Database.Load("db.json");

        static async Task Main(string[] args)
        {
            // init Bybit P2P API to conclude connections
            HttpController.sharedAPI = new APIClient(
                apiKey: cfg.API_KEY,
                apiSecret: cfg.API_SECRET,
                testnet: false
                );

            // init webserver for incoming requests
            InitWebServer(cfg.SERVE_URI);

            while (true)
            {
                // wasting CPU cycles
                await Task.Delay(1000);
            }
        }

        private static void InitWebServer(string url)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithCors()
                .WithStaticFolder("/public", "static", true)
                .WithWebApi("/api", HttpController.AsJSON, m => m
                    .WithController<HttpController>());

            server.HandleHttpException(async (context, exception) =>
            {
                context.Response.StatusCode = exception.StatusCode;

                switch (exception.StatusCode)
                {
                    case 404:
                        await context.SendStringAsync("Not Found", "text/html", Encoding.UTF8);
                        break;
                    case 400:
                        await context.SendStringAsync("Bad Request", "text/html", Encoding.UTF8);
                        break;
                    case 403:
                        await context.SendStringAsync("Forbidden", "text/html", Encoding.UTF8);
                        break;
                    case 429:
                        await context.SendStringAsync("Too Many Requests", "text/html", Encoding.UTF8);
                        break;
                    default:
                        await context.SendStringAsync("Internal Server Error", "text/html", Encoding.UTF8);
                        break;
                }
            });

            server.Start();
        }
    }
}
