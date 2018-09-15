using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BookFunction
{
    public static class TestFunction
    {
        [FunctionName("TestFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Orchestrator", "TedTalk");

            log.Info($"Started orchestration with ID = '{instanceId}'.");
                
            return starter.CreateCheckStatusResponse(req, instanceId);
        }        
        
    }
}
