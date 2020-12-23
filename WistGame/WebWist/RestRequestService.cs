using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Resources;

namespace WebWist
{
    public class RestRequestService
    {
        public static string HandleRestRequest(HttpContext context)
        {
            return GetIndexPage();
        }

        public static string GetIndexPage()
        {
            ResourceManager resourceManager = new ResourceManager("WebWist.Properties.Resources", typeof(Program).Assembly);
            string indexFile = resourceManager.GetString("GameIndex");

            string insert = string.Empty;
            string response = indexFile;

            return response;
        }
    }
}
