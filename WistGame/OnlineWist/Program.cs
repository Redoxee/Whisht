using System;
using System.Net;

using System.Resources;

namespace OnlineWist
{
    class Program
    {
        private static void Main(string[] args)
        {
            var ws = new WebServer(SendResponse, "http://localhost:8080/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        public static string SendResponse(HttpListenerContext context)
        {
            if (context.Request.IsWebSocketRequest)
            {
                return GetWebSocketResponse(context);
            }
            else
            {
                return GetHttpResponse(context);
            }
        }

        private static string GetHttpResponse(HttpListenerContext context)
        {
            ResourceManager resourceManager = new ResourceManager("OnlineWist.GameResources", typeof(Program).Assembly);
            string indexFile = (resourceManager.GetString("GameIndex"));
            return indexFile;
        }

        private static string GetWebSocketResponse(HttpListenerContext context)
        {
            Console.WriteLine("Web socket request");
            return "webSocketRequest";
        }
    }
}
