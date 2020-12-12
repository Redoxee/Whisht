using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialize game.
            GameProcess gp = GameProcess.Instance;
            gp.InitializeGame(2, 5);

            RestWorker.StartRestWorker(args);

            bool stop = false;
            while (!stop)
            {
                string line = Console.ReadLine();
                if (line.ToLower() == "quit")
                {
                    stop = true;
                }
            };
        }
    }
}
