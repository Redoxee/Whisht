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
        private const string InsertSerializationSymole = "$INSERT_SERIALIZATION";

        public static string HandleRestRequest(HttpContext context)
        {
            return GetIndexPage();
        }

        public static string GetIndexPage()
        {
            ResourceManager resourceManager = new ResourceManager("WebWist.Properties.Resources", typeof(Program).Assembly);
            string indexFile = resourceManager.GetString("GameIndex");
            string serializationCommon = resourceManager.GetString("Common_generated");
            string serializationWist = resourceManager.GetString("Wist_generated");

            string insert = serializationCommon + serializationWist;
            string response = indexFile.Replace(RestRequestService.InsertSerializationSymole, insert);

            return response;
        }
    }
}
