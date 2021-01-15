using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("Requestbody: {0}", requestBody);

            if (req.ContentType?.IndexOf(@"/xml", 0, System.StringComparison.OrdinalIgnoreCase) == -1)
            {
                return new BadRequestObjectResult(@"Content-Type header must be an XML content type");
            }

            XmlDocument doc = new XmlDocument();
            try {
                doc.LoadXml(requestBody);
                return new OkObjectResult(doc);
            }
            catch (System.IO.FileNotFoundException)
            {
                log.LogInformation("Exception: bad xml element");
                return new BadRequestObjectResult("Exception: bad xml element");
            }
            catch (Exception e)
            {
                log.LogInformation("Exception: {0}",e.Message);
                string message = "Content error " + e.Message;
                return new BadRequestObjectResult(message);
            }

        }
    }
}
