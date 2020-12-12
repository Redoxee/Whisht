using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebWist
{
    public class RestWorker
    {
        public static void StartRestWorker(string[] args)
        {
            Thread thread = new Thread(RestWork);
            thread.Start(args);
        }

        private static void RestWork(object arguments)
        {
            string[] args = (string[])arguments;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
