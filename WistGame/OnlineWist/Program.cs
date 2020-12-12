using System;
using System.Net;

using System.Resources;

namespace OnlineWist
{
    class Program
    {
        public static string SendResponse(HttpListenerRequest request)
        {
            ResourceManager resourceManager = new ResourceManager("OnlineWist.GameResources", typeof(Program).Assembly);
            string indexFile = (resourceManager.GetString("GameIndex"));
            return indexFile;
        }

        private static void Main(string[] args)
        {
            var ws = new WebServer(SendResponse, "http://localhost:8080/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }
    }
}
