using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Resources;
using System.Net.WebSockets;
using System.Threading;

namespace WebWist
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", this.defaultEndPoint);
                endpoints.MapGet("/Game", this.gameEndPoint);
            });
        }

        private async Task defaultEndPoint(HttpContext context)
        {
            ResourceManager resourceManager = new ResourceManager("WebWist.GameResources",typeof(Startup).Assembly);
            string indexFile = (resourceManager.GetString("GameIndex"));
            await context.Response.WriteAsync(indexFile);
        }

        private async Task gameEndPoint(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                System.Console.WriteLine("Recieving web socket data.");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
